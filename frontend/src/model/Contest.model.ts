import type { RankingOrder } from "./RankingOrder.model";
import type { RankingType } from "./RankingType.model";
import type { ScoreEntry } from "./ScoreEntry.model";
import type { ScoreType } from "./ScoreType.model";

export interface Contest {
  id: string;
  name: string;
  admin: string[];
  createdDate: Date;
  description?: string;
  active: boolean;
  rankingType: RankingType;
  rankingOrder: RankingOrder;
  contestants: string[];
  scoreType: ScoreType;
  displayedScores: ScoreEntry[];
}
