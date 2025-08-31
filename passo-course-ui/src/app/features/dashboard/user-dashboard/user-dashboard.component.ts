import { Component } from '@angular/core';
import { EnrollmentService } from '../../../core/services/enrollment.service';
import { ProgressService } from '../../../core/services/progress.service';
import { CourseResponse } from '../../../core/models/course.models';
import { ProgressResponse } from '../../../core/models/progress.models';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-user-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule, MatIconModule, MatButtonModule,
    MatProgressBarModule, MatChipsModule, MatDividerModule, RouterLink
  ],
  templateUrl: './user-dashboard.component.html',
  styleUrls: ['./user-dashboard.component.scss']
})
export class UserDashboardComponent {
  courses: CourseResponse[] = [];
  progress: ProgressResponse[] = [];

  constructor(private enrollmentService: EnrollmentService, private prog: ProgressService) {}

  ngOnInit(){
    this.enrollmentService.myEnrollments().subscribe(r => this.courses = r);
    this.prog.getMine().subscribe(r => this.progress = r);
  }

  unenroll(id: string){
    this.enrollmentService.unenroll(id).subscribe({
      next: () => { this.courses = this.courses.filter(x => x.id !== id); this.progress = this.progress.filter(p => p.courseId !== id); }
    });
  }

  progressBy(courseId: string) {
    const p = this.progress.find(x => x.courseId === courseId);
    if (!p) return 0;
    const total = Math.max(1, (p.totalLessons ?? 0) + (p.totalQuizzes ?? 0));
    const done = (p.completedLessons ?? 0) + (p.completedQuizzes ?? 0);
    const pct = Math.round((done * 100) / total);
    return Math.min(100, Math.max(0, pct));
  }

  bump(courseId: string, kind: 'lesson'|'quiz') {
    const p = this.progress.find(x => x.courseId === courseId);
    if (!p) return;
    if (kind === 'lesson') p.completedLessons += 1; else p.completedQuizzes += 1;
  }
}