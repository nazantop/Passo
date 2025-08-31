export type Difficulty = 'Beginner'|'Intermediate'|'Advanced';

export interface LessonItem { 
    id: string; 
    title: string; 
    order: number; }

export interface CourseResponse { 
    id: string; 
    title: string; 
    description: string; 
    duration: number; 
    difficulty: Difficulty | string; 
    instructorId: string; 
    instructorEmail: string; 
    createdAt: string; 
    instructorFullName: string;
    totalLessons: number;
    totalQuizzes: number;
    lessons: LessonItem[];
    completedLessonIds: string[];
    completedQuizIndices: number[];
    percent: number;
}

export interface CourseCreateRequest { 
    title: string; 
    description: string; 
    duration: number; 
    difficulty: number; 
}

export interface CourseUpdateRequest {
    title?: string; 
    description?: string; 
    duration?: number; 
    difficulty?: number; 
}