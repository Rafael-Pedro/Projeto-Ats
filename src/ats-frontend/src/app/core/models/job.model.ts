export interface Job {
  id: string;
  title: string;
  description: string;
  salary?: number;
  isActive: boolean;
  createdAt: string;
}