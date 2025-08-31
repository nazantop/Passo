export interface AuthResponse { 
    userId: string; 
    email: string; 
    roles: string[]; 
    accessToken: string; 
    expiresAt: string; 
    firstName: string;
    lastName: string;
    fullName: string; 
}

export interface LoginRequest { 
    email: string; 
    password: string; 
}

export interface RegisterRequest { 
    email: string; 
    password: string; 
    role: 'User'|'Instructor'; 
    firstName: string;
    lastName: string;
}