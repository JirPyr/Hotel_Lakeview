export type Room = {
  id: number;
  roomNumber: string;
  roomType: number;
  maxGuests: number;
  basePricePerNight: number;
  description: string | null;
  isActive: boolean;
};

export type PagedResult<T> = {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
};