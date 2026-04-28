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