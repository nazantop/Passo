import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.models';


const TOKEN_KEY = 'passo.token';
const USER_KEY = 'passo.user';


@Injectable({ providedIn: 'root' })
export class AuthService {
  
  constructor(private http: HttpClient, private router: Router, private snack: MatSnackBar) {}


  login(payload: LoginRequest){ 
    return this.http.post<AuthResponse>(`${environment.apiBaseUrl}/auth/login`, payload); 
  }

  register(payload: RegisterRequest){ 
    return this.http.post<AuthResponse>(`${environment.apiBaseUrl}/auth/register`, payload); 
  }

  setSession(resp: AuthResponse){ 
    localStorage.setItem(TOKEN_KEY, resp.accessToken); 
    localStorage.setItem(USER_KEY, JSON.stringify(resp)); 
  }

  logout() {
    localStorage.removeItem('passo.token');
    localStorage.removeItem('passo.user');
    this.snack.open('Logged out', 'Close', { duration: 1500 });
    this.router.navigateByUrl('/welcome');
}

  getToken(){ 
    return localStorage.getItem(TOKEN_KEY); 
  }

  getUser(): AuthResponse | null { 
    const raw = localStorage.getItem(USER_KEY); 
    return raw ? JSON.parse(raw) : null;
  }

  isLoggedIn(){ 
    return !!this.getToken(); 
  }

  hasRole(role: string){ 
    return this.getUser()?.roles?.includes(role) ?? false; 
  }
}