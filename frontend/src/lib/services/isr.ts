import { MemoryCache, type ICache } from "./cache";

class IsrService implements ICache<Response> {
  constructor(private readonly cache: ICache<Response>) {}

  get(key: string): Response | undefined {
    console.log(`[isr] get ${key}`);
    const result = this.cache.get(key);
    if (!result) return undefined;
    // Clone the response so that we don't modify the original.
    return result.clone();
  }

  set(key: string, value: Response, ttl: number): void {
    console.log(`[isr] set ${key}`);
    // Clone the response so that we don't modify the original.
    this.cache.set(key, value.clone(), ttl);
  }

  del(key: string): void {
    console.log(`[isr] del ${key}`);
    this.cache.del(key);
  }
}

export function invalidate(key: string) {
  isr.del(key);
}

const cache = new MemoryCache<Response>();
export const isr = new IsrService(cache);
