import { computed, inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Cart, CartItem, Coupon } from '../../shared/models/cart';
import { Product } from '../../shared/models/product';
import { firstValueFrom, map, switchMap, tap } from 'rxjs';
import { DeliveryMethod } from '../../shared/models/deliveryMethod';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  cart = signal<Cart | null>(null);
  itemCount = computed(() => this.getCartItemCount());
  selectedDelivery = signal<DeliveryMethod | null>(null);
  coupon = signal<{ percentOff: number } | null>(null); // Store coupon info

  totals = computed(() => {
    const subtotal = this.getCartTotal();
    const delivery = this.selectedDelivery();
    const shipping = delivery ? delivery.price : 0;
    const discount = this.coupon()?.percentOff ? (subtotal * this.coupon()!.percentOff / 100) : 0; // Calculate discount based on coupon
    return {
      subtotal,
      shipping,
      discount,
      total: subtotal + shipping - discount
    }
  });

  getCart(id: string) {
    return this.http.get<Cart>(this.baseUrl + "cart?id=" + id).pipe(
      map(cart => {
        this.cart.set(cart);
        return cart;
      })
    );
  }

  getCartItemCount(): number {
    return this.cart()?.items.reduce((sum, item) => sum + item.quantity, 0) || 0;
  }

  getCartTotal(): number {
    return this.cart()?.items.reduce((sum, item) => sum + item.price * item.quantity, 0) || 0;
  }

  setCart(cart: Cart) {
    return this.http.post<Cart>(this.baseUrl + 'cart', cart).pipe(
      tap(cart => {
        this.cart.set(cart)
      })
    )
  }

  async addItemToCart(item: CartItem | Product, quantity = 1) {
    const cart = this.cart() ?? this.createCart();
    if (this.isProduct(item)) {
      item = this.mapProductToCartItem(item);
    }
    cart.items = this.addOrUpdateItem(cart.items, item, quantity);
    await firstValueFrom(this.setCart(cart));
  }

  removeItemFromCart(productId: number, quantity = 1) {
    const cart = this.cart();

    if (!cart) return;

    const index = cart.items.findIndex(x => x.productId === productId);

    if (index !== -1) {
      if (cart.items[index].quantity > quantity) {
        cart.items[index].quantity -= quantity;
      }
      else {
        cart.items.splice(index, 1);
      }
      if (cart.items.length === 0) {
        this.deleteCart();
      }
      else {
        this.setCart(cart);
      }
    }
  }

  deleteCart() {
    this.http.delete(this.baseUrl + 'cart?id=' + this.cart()?.id).subscribe({
      next: () => {
        localStorage.removeItem('cart_id');
        this.cart.set(null);
      }
    });
  }

  applyDiscount(code: string) {
    return this.http.get<Coupon>(this.baseUrl + 'coupons/' + code).pipe(
      map(coupon => {
        this.coupon.set({ percentOff: coupon.percentOff });
        return coupon;
      })
    );
  }
  

  private addOrUpdateItem(items: CartItem[], item: CartItem, quantity: number): CartItem[] {
    const index = items.findIndex(x => x.productId === item.productId);
    if (index === -1) {
      item.quantity = quantity;
      items.push(item);
    }
    else {
      items[index].quantity += quantity;
    }
    return items;
  }

  private mapProductToCartItem(item: Product): CartItem {
    return {
      productId: item.id,
      productName: item.name,
      price: item.price,
      quantity: 0,
      pictureUrl: item.pictureUrl,
      brand: item.brand,
      type: item.type
    };
  }

  private isProduct(item: CartItem | Product): item is Product {
    return (item as Product).id !== undefined;
  }

  private createCart(): Cart {
    const cart = new Cart();
    localStorage.setItem("cart_id", cart.id);
    return cart;
  }
}
