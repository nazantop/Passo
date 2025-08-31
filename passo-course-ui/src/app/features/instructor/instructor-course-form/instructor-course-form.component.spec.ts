import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstructorCourseFormComponent } from './instructor-course-form.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';

describe('InstructorCourseForm', () => {
  let component: InstructorCourseFormComponent;
  let fixture: ComponentFixture<InstructorCourseFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
     imports: [InstructorCourseFormComponent, HttpClientTestingModule, RouterTestingModule]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InstructorCourseFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
