import { Component, ViewChild } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CourseService } from '../../../core/services/course.service';
import { CourseResponse } from '../../../core/models/course.models';
import { CommonModule } from '@angular/common';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-instructor-courses',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTableModule,
    MatIconModule,
    MatCardModule,
    MatChipsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatPaginatorModule,
    MatSortModule,
    MatTooltipModule
  ],
  templateUrl: './instructor-courses.component.html',
  styleUrls: ['./instructor-courses.component.scss']
})
export class InstructorCoursesComponent {
  displayedColumns: string[] = ['title', 'difficulty', 'duration', 'totals', 'createdAt', 'actions'];
  dataSource = new MatTableDataSource<CourseResponse>([]);
  filterValue = '';

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(private courseService: CourseService, private router: Router, private snack: MatSnackBar) {}

  ngOnInit() {
    this.courseService.getAllByInstructorId().subscribe(r => {
      this.dataSource.data = r;
      this.dataSource.filterPredicate = (c, f) => {
        const v = (c.title + ' ' + (c.description || '')).toLowerCase();
        return v.includes(f.trim().toLowerCase());
      };
    });
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  applyFilter(ev: Event) {
    this.filterValue = (ev.target as HTMLInputElement).value || '';
    this.dataSource.filter = this.filterValue;
    if (this.dataSource.paginator) this.dataSource.paginator.firstPage();
  }

  clearFilter() {
    this.filterValue = '';
    this.dataSource.filter = '';
    if (this.dataSource.paginator) this.dataSource.paginator.firstPage();
  }

  goNew() {
    this.router.navigateByUrl('/instructor/courses/new');
  }

  edit(id: string) {
    this.router.navigateByUrl(`/instructor/courses/edit/${id}`);
  }

  remove(id: string) {
    this.courseService.delete(id).subscribe({
      next: () => {
        this.snack.open('Deleted', 'Close', { duration: 1200 });
        this.dataSource.data = this.dataSource.data.filter(x => x.id !== id);
      },
      error: e => this.snack.open(e?.error || 'Failed', 'Close', { duration: 1500 })
    });
  }
}
