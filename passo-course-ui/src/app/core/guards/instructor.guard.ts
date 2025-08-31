import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const canActivateInstructor: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (!auth.isLoggedIn() || !auth.hasRole('Instructor')) { router.navigate(['/login']);
     return false; 
    }
  return true;
};