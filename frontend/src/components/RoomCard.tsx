import { Room } from "@/types/room";
import { RoomImage } from "@/types/roomImage";

type RoomCardProps = {
  room: Room;
  image?: RoomImage;
};

function getRoomTypeName(roomType: number): string {
  switch (roomType) {
    case 1:
      return "Economy";
    case 2:
      return "Standard";
    case 3:
      return "Superior";
    case 4:
      return "Junior Suite";
    case 5:
      return "Suite";
    default:
      return "Tuntematon";
  }
}

export function RoomCard({ room, image }: RoomCardProps) {
  const imageUrl = image?.filePathOrUrl;

  return (
    <article className="overflow-hidden rounded-2xl bg-white shadow-sm transition hover:-translate-y-1 hover:shadow-md">
      <div className="h-48 bg-slate-200">
        {imageUrl ? (
          <img
            src={imageUrl}
            alt={`Huone ${room.roomNumber}`}
            className="h-full w-full object-cover"
          />
        ) : (
          <div className="flex h-full items-center justify-center text-slate-500">
            Ei kuvaa
          </div>
        )}
      </div>

      <div className="p-5">
        <div className="mb-3 flex items-start justify-between gap-4">
          <div>
            <h2 className="text-xl font-bold text-slate-900">
              Huone {room.roomNumber}
            </h2>
            <p className="text-sm text-slate-500">
              {getRoomTypeName(room.roomType)}
            </p>
          </div>

          <span
            className={`rounded-full px-3 py-1 text-xs font-semibold ${
              room.isActive
                ? "bg-green-100 text-green-700"
                : "bg-red-100 text-red-700"
            }`}
          >
            {room.isActive ? "Aktiivinen" : "Pois käytöstä"}
          </span>
        </div>

        <p className="mb-4 line-clamp-2 text-sm text-slate-600">
          {room.description ?? "Ei kuvausta."}
        </p>

        <div className="flex items-end justify-between">
          <div>
            <p className="text-sm text-slate-500">Kapasiteetti</p>
            <p className="font-semibold text-slate-900">
              {room.maxGuests} henkilöä
            </p>
          </div>

          <div className="text-right">
            <p className="text-sm text-slate-500">Hinta</p>
            <p className="text-lg font-bold text-slate-900">
              {room.basePricePerNight} € / yö
            </p>
          </div>
        </div>
      </div>
    </article>
  );
}