import { Routes } from '@angular/router';
import { canActivateInstructor } from './core/guards/instructor.guard';
import { CoursesListComponent } from './features/courses/courses-list/courses-list.component';
import { CourseDetailComponent } from './features/courses/course-detail/course-detail.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { UserDashboardComponent } from './features/dashboard/user-dashboard/user-dashboard.component';
import { InstructorCoursesComponent } from './features/instructor/instructor-courses/instructor-courses.component';
import { InstructorCourseFormComponent } from './features/instructor/instructor-course-form/instructor-course-form.component';
import { WelcomeComponent } from './features/welcome/welcome.component';
import { canActivateUserOnly } from './core/guards/user.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'welcome' },
  { path: 'welcome', component: WelcomeComponent },

  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'courses', component: CoursesListComponent },
  { path: 'courses/:id', component: CourseDetailComponent },
  { path: 'dashboard', component: UserDashboardComponent, canActivate: [canActivateUserOnly] },

  {
    path: 'instructor',
    canActivate: [canActivateInstructor],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'courses' },
      { path: 'courses', component: InstructorCoursesComponent },
      { path: 'courses/new', component: InstructorCourseFormComponent },
      { path: 'courses/edit/:id', component: InstructorCourseFormComponent },
    ],
  },
  { path: '**', component: WelcomeComponent }
];
