import { CanActivateFn, Router } from '@angular/router';
import { CartService } from '../services/cart.service';
import { SnackbarService } from '../services/snackbar.service';
import { inject } from '@angular/core';

export const checkoutGuard: CanActivateFn = (route, state) => {
  const cartService = inject(CartService); 
  const router = inject(Router);
  const snackBarService = inject(SnackbarService);

  if (cartService.getCartItemCount() === 0) {
    snackBarService.error('Your cart is empty. Please add items before checking out.');
    router.navigate(['/cart']);
    return false;
  }

  return true;
};