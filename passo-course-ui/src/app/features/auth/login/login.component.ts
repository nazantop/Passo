import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';


@Component({
selector: 'app-login',
 imports: [
    CommonModule, ReactiveFormsModule, RouterLink,
    MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule
  ],
templateUrl: './login.component.html',
styleUrls: ['./login.component.scss']
})

export class LoginComponent {
  loading = false;
  private fb = inject(FormBuilder);  
  
  form = this.fb.group({ 
    email: ['', [Validators.required, Validators.email]], 
    password: ['', [Validators.required, Validators.minLength(6)]] });

  constructor(private auth: AuthService, private snack: MatSnackBar, private router: Router) {}

  submit(){
    if(this.form.invalid) return;
    this.loading = true;
    this.auth.login(this.form.value as any).subscribe({
      next: res => {
      this.auth.setSession(res);
      this.snack.open('Welcome!','Close',{duration:1200});
      if (res.roles?.includes('Instructor')) this.router.navigateByUrl('/instructor/courses');
      else this.router.navigateByUrl('/dashboard');
      },
      error: err => this.snack.open(err?.error || 'Login failed','Close',{duration:1500}),
      complete: () => this.loading = false
      });
  }
}