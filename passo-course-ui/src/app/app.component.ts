import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './shared/navbar/navbar.component';


@Component({
selector: 'app-root',
imports: [CommonModule, RouterOutlet, NavbarComponent],
templateUrl: './app.component.html',
styleUrls: ['./app.component.scss']
})
export class AppComponent {}