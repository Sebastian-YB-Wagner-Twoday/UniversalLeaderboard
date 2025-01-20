export interface Session {
  tokenType: string;
  accessToken: string;
  expiresIn: number;
  refreshToken: string;
}
