import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';


@Component({
selector: 'app-navbar',
imports: [CommonModule, RouterLink, MatToolbarModule, MatButtonModule, MatIconModule],
templateUrl: './navbar.component.html',
styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent {

    constructor(public auth: AuthService, 
        private router: Router) {}

    logout(){ 
        this.auth.logout(); 
        this.router.navigateByUrl('/');
 }
}