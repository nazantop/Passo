import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const canActivateUserOnly: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (!auth.isLoggedIn() || !auth.hasRole('User')) {
    router.navigate(['/login']);
    return false;
  }
  return true;
};