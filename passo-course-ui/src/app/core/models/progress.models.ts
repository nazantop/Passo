export interface ProgressResponse { 
    courseId: string;
    completedLessons: number;
    completedQuizzes: number;
    totalLessons: number;
    totalQuizzes: number;
    percent: number;
}