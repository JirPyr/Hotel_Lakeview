export type RoomImage = {
  id: number;
  roomId: number;
  fileName: string;
  filePathOrUrl: string;
  contentType: string;
  sortOrder: number;
  isPrimary: boolean;
  uploadedAtUtc: string;
};