import { Component, inject } from '@angular/core';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CourseService } from '../../../core/services/course.service';
import { CourseResponse } from '../../../core/models/course.models';
import { MatFormFieldModule } from '@angular/material/form-field';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBarModule } from '@angular/material/snack-bar';

@Component({
  selector: 'app-instructor-course-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule
  ],
  templateUrl: './instructor-course-form.component.html',
  styleUrls: ['./instructor-course-form.component.scss']
})
export class InstructorCourseFormComponent {
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private courseService: CourseService,
    private snack: MatSnackBar
  ) {}

  isEdit = false; id: string | null = null; loading = false;
  private fb = inject(FormBuilder);

  form: FormGroup = this.fb.group({
    title: ['', [Validators.required, Validators.minLength(3)]],
    description: ['', [Validators.required, Validators.minLength(8)]],
    duration: [10, [Validators.required, Validators.min(1)]],
    difficulty: [0, [Validators.required]],
    lessons: this.fb.array<FormGroup>([]),
    quizzes: this.fb.array<FormGroup>([])
  });

  get lessonsArray(): FormArray<FormGroup> {
    return this.form.get('lessons') as FormArray<FormGroup>;
  }
  get quizzesArray(): FormArray<FormGroup> {
    return this.form.get('quizzes') as FormArray<FormGroup>;
  }

  addLesson() {
    const order = this.lessonsArray.length + 1;
    this.lessonsArray.push(
      this.fb.group({
        title: ['', Validators.required],
        order: [order, Validators.required]
      })
    );
  }

  removeLesson(i: number) {
    this.lessonsArray.removeAt(i);
    this.reindexLessons();
  }

  addQuiz() {
    const order = this.quizzesArray.length + 1;
    this.quizzesArray.push(
      this.fb.group({
        title: ['', Validators.required],
        order: [order, Validators.required]
      })
    );
  }

  removeQuiz(i: number) {
    this.quizzesArray.removeAt(i);
    this.reindexQuizzes();
  }

  private reindexLessons() {
    this.lessonsArray.controls.forEach((g, i) => g.get('order')?.setValue(i + 1));
  }

  private reindexQuizzes() {
    this.quizzesArray.controls.forEach((g, i) => g.get('order')?.setValue(i + 1));
  }

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id');
    this.isEdit = !!this.id;
    if (this.isEdit && this.id) {
      this.courseService.getById(this.id).subscribe((c: CourseResponse | any) => {
        this.form.patchValue({
          title: c.title,
          description: c.description,
          duration: c.duration,
          difficulty: this.mapDifficultyToNumber(c.difficulty)
        });

        while (this.lessonsArray.length) this.lessonsArray.removeAt(0);
        const incomingLessons = c.lessons ?? [];
        incomingLessons.sort((a: any, b: any) => (a.order ?? 0) - (b.order ?? 0))
          .forEach((l: any) => {
            this.lessonsArray.push(
              this.fb.group({
                title: [l.title || '', Validators.required],
                order: [l.order || this.lessonsArray.length + 1, Validators.required]
              })
            );
          });

        while (this.quizzesArray.length) this.quizzesArray.removeAt(0);
        const incomingQuizzes = c.quizzes ?? c.quizes ?? [];
        incomingQuizzes.sort((a: any, b: any) => (a.order ?? 0) - (b.order ?? 0))
          .forEach((q: any) => {
            this.quizzesArray.push(
              this.fb.group({
                title: [q.title || '', Validators.required],
                order: [q.order || this.quizzesArray.length + 1, Validators.required]
              })
            );
          });
      });
    }
  }

  save() {
    if (this.form.invalid) return;
    this.loading = true;

    const val = {
      ...this.form.value,
      totalLessons: this.lessonsArray.length,
      totalQuizzes: this.quizzesArray.length
    } as any;

    if (this.isEdit && this.id) {
      this.courseService.update(this.id, val).subscribe({
        next: () => { this.snack.open('Updated', 'Close', { duration: 1200 }); this.router.navigateByUrl('/instructor/courses'); },
        error: e => this.snack.open(e?.error || 'Failed', 'Close', { duration: 1500 }),
        complete: () => this.loading = false
      });
    } else {
      this.courseService.create(val).subscribe({
        next: () => { this.snack.open('Created', 'Close', { duration: 1200 }); this.router.navigateByUrl('/instructor/courses'); },
        error: e => this.snack.open(e?.error || 'Failed', 'Close', { duration: 1500 }),
        complete: () => this.loading = false
      });
    }
  }

  private mapDifficultyToNumber(d: string | number) {
    if (typeof d === 'number') return d;
    switch (d) {
      case 'Beginner': return 0;
      case 'Intermediate': return 1;
      case 'Advanced': return 2;
      default: return 0;
    }
  }
}
