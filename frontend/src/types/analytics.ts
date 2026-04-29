export type ReservationSummaryReport = {
  startDate: string;
  endDate: string;
  activeReservationCount: number;
  cancelledReservationCount: number;
  totalReservationCount: number;
  cancellationRatePercentage: number;
  cancelledRevenueValue: number;
};

export type OccupancyReport = {
  startDate: string;
  endDate: string;
  activeRoomCount: number;
  totalDays: number;
  totalAvailableRoomNights: number;
  occupiedRoomNights: number;
  occupancyRatePercentage: number;
};

export type MonthlyRevenue = {
  year: number;
  month: number;
  revenue: number;
};

export type RevenueReport = {
  startDate: string;
  endDate: string;
  months: MonthlyRevenue[];
  totalRevenue: number;
};

export type PopularRoomType = {
  roomType: number;
  reservationCount: number;
  percentageOfReservations: number;
};

export type PopularRoomTypesReport = {
  startDate: string;
  endDate: string;
  totalReservationCount: number;
  roomTypes: PopularRoomType[];
};
// Admin näkymä - Asiakkaat ja varaukset
export type AdminCustomer = {
  id: number;
  fullName: string;
  email: string;
  phoneNumber: string;
  notes?: string;
};

export type AdminReservation = {
  id: number;
  customerId: number;
  customerName: string;
  roomId: number;
  roomNumber: string;
  checkInDate: string;
  checkOutDate: string;
  guestCount: number;
  notes?: string;
  status: string;
};

export type CustomersReport = {
  items: AdminCustomer[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
};

export type ReservationsReport = {
  items: AdminReservation[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
};