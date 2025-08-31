import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';


@Component({
  selector: 'app-register',
    imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule
  ],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})

export class RegisterComponent {
  loading = false;
  private fb = inject(FormBuilder);  
  form = this.fb.group({
    firstName: ['', [Validators.required, Validators.minLength(2)]],
    lastName:  ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    role: ['User', Validators.required]
  });

  constructor(private auth: AuthService, 
    private snack: MatSnackBar, 
    private router: Router) {}


  submit(){
    if(this.form.invalid) return;
    this.loading = true;
    this.auth.register(this.form.value as any).subscribe({
      next: res => {
        this.auth.setSession(res);
        this.snack.open('Account created!','Close',{duration:1200});
        if (res.roles?.includes('Instructor')) this.router.navigateByUrl('/instructor/courses');
        else this.router.navigateByUrl('/dashboard');
      },
      error: err => this.snack.open(err?.error || 'Register failed','Close',{duration:1500}),
      complete: () => this.loading = false
    });
   }
}