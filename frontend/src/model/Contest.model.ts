import type { RankingOrder } from "./RankingOrder.model";
import type { RankingType } from "./RankingType.model";
import type { ScoreEntry } from "./ScoreEntry.model";

export interface Contest {
  Id: string;
  name: string;
  admin: string[];
  createdDate: Date;
  description?: string;
  active: boolean;
  rankingType: RankingType;
  rankingOrder: RankingOrder;
  Contestants: string[];
  ScoreEntries: ScoreEntry<number>[];
}
