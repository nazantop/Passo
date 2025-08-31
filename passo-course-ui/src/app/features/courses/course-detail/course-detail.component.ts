import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CourseService } from '../../../core/services/course.service';
import { EnrollmentService } from '../../../core/services/enrollment.service';
import { AuthService } from '../../../core/services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CourseResponse } from '../../../core/models/course.models';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { HttpErrorResponse } from '@angular/common/http';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { ProgressService } from '../../../core/services/progress.service';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
  selector: 'app-course-detail',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatDividerModule,
    MatIconModule,
    MatChipsModule,
    MatButtonModule,
    MatProgressBarModule
  ],
  templateUrl: './course-detail.component.html',
  styleUrls: ['./course-detail.component.scss']
})
export class CourseDetailComponent {
  course?: CourseResponse;
  completedLessons = new Set<string>();
  completedQuizzes = new Set<number>();
  enrolled = false;

  constructor(
    private route: ActivatedRoute,
    private courseService: CourseService,
    private progressService: ProgressService,
    private enrollmentService: EnrollmentService,
    public authService: AuthService,
    private snack: MatSnackBar
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.courseService.getById(id).subscribe(r => this.course = r);
    this.courseService.getOutline(id).subscribe(o => {
      this.course = o;
      this.completedLessons = new Set(o.completedLessonIds);
      this.completedQuizzes = new Set(o.completedQuizIndices);
      this.loadEnrollment();
    });
  }

  loadEnrollment() {
    if (!this.course || !this.authService.isLoggedIn()) { this.enrolled = false; return; }
    this.enrollmentService.isEnrolled(this.course.id).subscribe(v => this.enrolled = !!v);
  }

  doEnroll() {
    if (!this.course) return;
    this.enrollmentService.enroll({ courseId: this.course.id }).subscribe({
      next: () => { this.enrolled = true; this.snack.open('Enrolled!', 'Close', { duration: 1200 }); },
      error: (err: HttpErrorResponse) => {
        const message =
          typeof err.error === 'string' && /Already enrolled/i.test(err.error) ? 'You are already enrolled in this course' :
          typeof err.error === 'string' && /Instructors cannot enroll/i.test(err.error) ? 'Instructors cannot enroll in courses' :
          (err.error?.message || err.error || 'Enrollment failed');
        this.snack.open(message, 'Close', { duration: 2000 });
      }
    });
  }

  doUnenroll() {
    if (!this.course) return;
    this.enrollmentService.unenroll(this.course.id).subscribe({
      next: () => { this.enrolled = false; this.snack.open('Unenrolled', 'Close', { duration: 1200 }); },
      error: () => this.snack.open('Unenroll failed', 'Close', { duration: 1500 })
    });
  }

  isCompleted(): boolean {
    if (!this.course) return false;
    const done = this.completedLessons.size + this.completedQuizzes.size;
    return done >= (this.course.totalLessons + this.course.totalQuizzes);
  }

 completeLesson(lessonId: string) {
  if (!this.course || this.completedLessons.has(lessonId)) return;

  this.progressService.completeLesson(lessonId).subscribe({
    next: () => {
      this.completedLessons.add(lessonId);
      this.course!.percent = this.calcPercent();
      this.snack.open('Lesson completed', 'Close', { duration: 1200 });
    },
    error: e => this.snack.open(e?.error.message || 'You need to enroll first', 'Close', { duration: 1500 }),
    complete: () => {}
  });
}

completeQuiz(idx: number) {
  if (!this.course || this.completedQuizzes.has(idx)) return;

  this.progressService.completeQuiz(this.course.id, idx).subscribe({
    next: () => {
      this.completedQuizzes.add(idx);
      this.course!.percent = this.calcPercent();
      this.snack.open('Quiz completed', 'Close', { duration: 1200 });
    },
    error: e => this.snack.open(e?.error.message || 'You need to enroll first', 'Close', { duration: 1500 }),
    complete: () => {}
  });
}


  calcPercent(): number {
    if (!this.course) return 0;
    const total = Math.max(1, this.course.totalLessons + this.course.totalQuizzes);
    const done = this.completedLessons.size + this.completedQuizzes.size;
    return Math.round((done * 100) / total);
  }
}
