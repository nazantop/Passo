import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstructorCoursesComponent } from './instructor-courses.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';

describe('InstructorCourses', () => {
  let component: InstructorCoursesComponent;
  let fixture: ComponentFixture<InstructorCoursesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InstructorCoursesComponent, HttpClientTestingModule, RouterTestingModule]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InstructorCoursesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
