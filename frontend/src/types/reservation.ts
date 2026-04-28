export type AvailableRoom = {
  id: number;
  roomNumber: string;
  roomType: number;
  maxGuests: number;
  basePricePerNight: number;
  description: string | null;
};

export type CreateReservationRequest = {
  customerId: number;
  roomId: number;
  checkInDate: string;
  checkOutDate: string;
  guestCount: number;
  notes?: string | null;
};

export type Reservation = {
  id: number;
  customerId: number;
  roomId: number;
  checkInDate: string;
  checkOutDate: string;
  guestCount: number;
  totalPrice: number;
  status: number;
  notes: string | null;
};
export type UpdateReservationRequest = {
  id: number;
  roomId: number;
  checkInDate: string;
  checkOutDate: string;
  guestCount: number;
  notes?: string | null;
};