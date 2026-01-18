export interface Candidate {
  id: string;
  name: string;
  email: string;
  age: number;
  resume?: string;
  createdAt: string;
  isDeleted: boolean;
}