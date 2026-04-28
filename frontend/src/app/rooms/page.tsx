"use client";
import BackToHomeLink from "@/components/BackToHomeLink";
import { useEffect, useState } from "react";
import { RoomCard } from "@/components/RoomCard";
import { getRoomImages } from "@/services/roomImageService";
import { getRooms } from "@/services/roomService";
import { Room, PagedResult } from "@/types/room";
import { RoomImage } from "@/types/roomImage";
import { uploadRoomImage } from "@/services/roomImageService";
import AuthGuard from "@/components/AuthGuard";
const PAGE_SIZE = 9;

export default function RoomsPage() {
  const [pagedRooms, setPagedRooms] = useState<PagedResult<Room> | null>(null);
  const [roomImages, setRoomImages] = useState<Record<number, RoomImage | undefined>>({});
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [selectedRoom, setSelectedRoom] = useState<Room | null>(null);
const [selectedFile, setSelectedFile] = useState<File | null>(null);
const [isUploading, setIsUploading] = useState(false);
const [uploadMessage, setUploadMessage] = useState("");

  useEffect(() => {
    async function fetchRooms() {
      setLoading(true);
      setError("");

      try {
        const roomsResult = await getRooms(page, PAGE_SIZE);
        setPagedRooms(roomsResult);

        const imageResults = await Promise.all(
          roomsResult.items.map(async (room) => {
            const images = await getRoomImages(room.id);
            const primaryImage =
              images.find((image) => image.isPrimary) ?? images[0];

            return [room.id, primaryImage] as const;
          })
        );

        setRoomImages(Object.fromEntries(imageResults));
      } catch (error) {
        console.error("Failed to fetch rooms", error);
        setError("Huoneiden lataaminen epäonnistui.");
      } finally {
        setLoading(false);
      }
    }

    fetchRooms();
  }, [page]);

  function goToPreviousPage() {
    setPage((currentPage) => Math.max(1, currentPage - 1));
  }

  function goToNextPage() {
    if (!pagedRooms?.hasNextPage) {
      return;
    }

    setPage((currentPage) => currentPage + 1);
  }
  async function handleUpload() {
  if (!selectedRoom || !selectedFile) return;

  try {
    setIsUploading(true);
    setUploadMessage("");

    await uploadRoomImage(
      selectedRoom.id,
      selectedFile,
      0,
      true
    );

    setUploadMessage("Kuva lisätty onnistuneesti!");
    setSelectedFile(null);
    setSelectedRoom(null);

    // REFRESH
    const images = await getRoomImages(selectedRoom.id);
    const primary = images.find(i => i.isPrimary) ?? images[0];

    setRoomImages(prev => ({
      ...prev,
      [selectedRoom.id]: primary
    }));

  } catch (err) {
    setUploadMessage(
      err instanceof Error ? err.message : "Upload epäonnistui"
    );
  } finally {
    setIsUploading(false);
  }
}
return (
  <AuthGuard>
  <main className="min-h-screen bg-slate-100 p-6">
    <div className="mx-auto max-w-7xl">
      <BackToHomeLink />

      <div className="mb-8 flex flex-col justify-between gap-4 md:flex-row md:items-end">
        <div>
          <h1 className="text-3xl font-bold text-slate-900">Huoneet</h1>
          <p className="mt-2 text-slate-600">
            Hallitse hotellin huoneita ja niiden kuvia.
          </p>
        </div>

        {pagedRooms && (
          <div className="rounded-xl bg-white px-4 py-3 text-sm text-slate-600 shadow-sm">
            Näytetään sivu {pagedRooms.page} / {pagedRooms.totalPages} ·
            yhteensä {pagedRooms.totalCount} huonetta
          </div>
        )}
      </div>

      {loading && (
        <div className="rounded-2xl bg-white p-8 text-slate-600 shadow-sm">
          Ladataan huoneita...
        </div>
      )}

      {error && (
        <div className="rounded-2xl bg-red-50 p-8 text-red-700 shadow-sm">
          {error}
        </div>
      )}

      {!loading && !error && pagedRooms && (
        <>
          <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
            {pagedRooms.items.map((room) => (
              <div key={room.id} className="space-y-2">
                <RoomCard room={room} image={roomImages[room.id]} />

                <button
                  type="button"
                  onClick={() => setSelectedRoom(room)}
                  className="w-full rounded-lg bg-slate-900 px-3 py-2 text-sm font-semibold text-white hover:bg-slate-700"
                >
                  Lisää kuva
                </button>
              </div>
            ))}
          </div>

          <div className="mt-8 flex items-center justify-between rounded-2xl bg-white p-4 shadow-sm">
            <button
              type="button"
              onClick={goToPreviousPage}
              disabled={!pagedRooms.hasPreviousPage}
              className="rounded-lg bg-slate-900 px-4 py-2 font-semibold text-white disabled:cursor-not-allowed disabled:bg-slate-300"
            >
              Edellinen
            </button>

            <p className="text-sm text-slate-600">
              Sivu {pagedRooms.page} / {pagedRooms.totalPages}
            </p>

            <button
              type="button"
              onClick={goToNextPage}
              disabled={!pagedRooms.hasNextPage}
              className="rounded-lg bg-slate-900 px-4 py-2 font-semibold text-white disabled:cursor-not-allowed disabled:bg-slate-300"
            >
              Seuraava
            </button>
          </div>
        </>
      )}

      {selectedRoom && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
          <div className="w-full max-w-md rounded-2xl bg-white p-6 shadow-xl">
            <h2 className="text-xl font-bold text-slate-900">
              Lisää kuva huoneelle {selectedRoom.roomNumber}
            </h2>

            <p className="mt-2 text-sm text-slate-600">
              Valitse kuva, joka lähetetään backendin kautta Azure Blob
              Storageen.
            </p>

            <input
              type="file"
              accept="image/*"
              onChange={(event) =>
                setSelectedFile(event.target.files?.[0] ?? null)
              }
              className="mt-5 w-full rounded-lg border border-slate-200 p-3 text-slate-700"
            />

            {uploadMessage && (
              <p className="mt-4 text-sm text-slate-600">{uploadMessage}</p>
            )}

            <div className="mt-6 flex justify-end gap-3">
              <button
                type="button"
                onClick={() => {
                  setSelectedRoom(null);
                  setSelectedFile(null);
                  setUploadMessage("");
                }}
                className="rounded-lg bg-slate-200 px-4 py-2 font-semibold text-slate-800 hover:bg-slate-300"
              >
                Peruuta
              </button>

              <button
                type="button"
                onClick={handleUpload}
                disabled={!selectedFile || isUploading}
                className="rounded-lg bg-slate-900 px-4 py-2 font-semibold text-white hover:bg-slate-700 disabled:cursor-not-allowed disabled:bg-slate-300"
              >
                {isUploading ? "Lähetetään..." : "Tallenna kuva"}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  </main>
  </AuthGuard>
);
}