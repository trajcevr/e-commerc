import { Component, inject } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { RouterLink } from '@angular/router';
import { CartService } from '../../../core/services/cart.service';
import { CommonModule, CurrencyPipe, Location } from '@angular/common';
import { Observable } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-order-summary',
  imports: [
    RouterLink,
    MatButton,
    MatFormField,
    MatLabel,
    MatInput,
    CurrencyPipe,
    FormsModule,
    MatIcon,
    CommonModule
  ],
  templateUrl: './order-summary.component.html',
  styleUrl: './order-summary.component.scss'
})
export class OrderSummaryComponent {
  cartService = inject(CartService);
  location = inject(Location);
  couponCode: string = '';  

  get couponApplied() {
    return this.cartService.coupon() !== null;
  }
  
  applyCouponCode(): void {
    if (this.couponCode.trim()) {
      this.cartService.applyDiscount(this.couponCode).subscribe({
        next: (coupon) => {
          console.log('Coupon applied:', coupon);
        },
        error: (err) => {
          console.error('Coupon application failed:', err);
        }
      });
    }
  }

  removeCouponCode(): void {
    this.cartService.coupon.set(null); 
    console.log('Coupon removed');
  }
}
