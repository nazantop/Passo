import { Component } from '@angular/core';
import { CourseService } from '../../../core/services/course.service';
import { CourseResponse } from '../../../core/models/course.models';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-courses-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatFormFieldModule,
    MatInputModule,
    MatPaginatorModule
  ],
  templateUrl: './courses-list.component.html',
  styleUrls: ['./courses-list.component.scss']
})
export class CoursesListComponent {
  courses: CourseResponse[] = [];
  filter = '';
  pageIndex = 0;
  pageSize = 9;

  get filtered(): CourseResponse[] {
    if (!this.filter) return this.courses;
    const f = this.filter.trim().toLowerCase();
    return this.courses.filter(c =>
      (c.title + ' ' + (c.description || '') + ' ' + (c.instructorFullName || c.instructorEmail || '')).toLowerCase().includes(f)
    );
  }

  get paged(): CourseResponse[] {
    const start = this.pageIndex * this.pageSize;
    return this.filtered.slice(start, start + this.pageSize);
  }

  constructor(private courseService: CourseService) {}

  ngOnInit() {
    this.courseService.getAll().subscribe(r => this.courses = r);
  }

  onSearch(e: Event) {
    this.filter = (e.target as HTMLInputElement).value || '';
    this.pageIndex = 0;
  }

  clear() {
    this.filter = '';
    this.pageIndex = 0;
  }

  onPage(ev: PageEvent) {
    this.pageIndex = ev.pageIndex;
    this.pageSize = ev.pageSize;
  }
}
