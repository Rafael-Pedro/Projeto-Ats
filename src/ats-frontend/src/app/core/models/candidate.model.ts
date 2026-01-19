export interface Candidate {
  id: string;
  name: string;
  email: string;
  age: number;  
  linkedIn?: string;
  resumeFileName?: string;  
  createdAt: string;
  isDeleted: boolean;
}