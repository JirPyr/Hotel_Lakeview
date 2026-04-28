// frontend/src/components/BackToHomeLink.tsx

import Link from "next/link";

export default function BackToHomeLink() {
  return (
    <div className="mx-auto mb-6 flex w-full max-w-6xl">
      <Link
        href="/"
        className="rounded-full border border-white/30 bg-white/80 px-5 py-2 text-sm font-semibold text-slate-800 shadow-sm backdrop-blur transition hover:bg-white"
      >
        ← Takaisin etusivulle
      </Link>
    </div>
  );
}