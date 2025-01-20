export interface ScoreEntry {
  id: string;
  userId: string;
  contestId: string;
  userName: string;
  score: number;
  relatedScoreEntries: string[];
  date: Date;
}
