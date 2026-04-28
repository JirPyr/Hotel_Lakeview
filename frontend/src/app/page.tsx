import Link from "next/link";
import AuthGuard from "@/components/AuthGuard";
import LogoutButton from "@/components/LogOutButton";

const quickActions = [
  {
    title: "Uusi varaus",
    description: "Hae vapaat huoneet ja luo asiakkaalle uusi varaus.",
    href: "/booking",
  },
  {
    title: "Huoneet",
    description: "Katso hotellin huoneet, kuvat ja huonetyypit.",
    href: "/rooms",
  },
  {
    title: "Varaukset",
    description: "Tarkastele ja hallitse olemassa olevia varauksia.",
    href: "/reservations",
  },
  {
    title: "Hallinta",
    description: "Raportit, käyttäjät ja johdon työkalut.",
    href: "/admin",
  },
];

export default function HomePage() {
  return (
    <AuthGuard>
      <main className="min-h-screen bg-gradient-to-b from-[#edf1f4] via-[#e8edf0] to-[#d8d0c2] p-6">
        <div className="mx-auto max-w-6xl">
          <section className="relative h-[300px] w-full overflow-hidden rounded-t-3xl shadow-sm">
            <img
              src="/hotellakeview.png"
              alt="Hotel Lakeview"
              className="absolute inset-0 h-full w-full object-cover"
            />

            <div className="absolute inset-0 bg-gradient-to-b from-black/10 via-[#2f2b24]/45 to-[#162033]" />

            <div className="relative z-10 flex h-full flex-col justify-between p-6 text-white">
              <div className="flex justify-end">
                <LogoutButton />
              </div>

              <div>
                <h1 className="text-3xl font-bold md:text-4xl">
                  Hotel Lakeview
                </h1>
                <p className="mt-2 text-sm text-slate-200 md:text-base">
                  Varausten, huoneiden ja asiakashallinnan työpöytä.
                </p>
              </div>
            </div>
          </section>

          <section className="mb-8 rounded-b-3xl bg-gradient-to-br from-[#162033] via-[#243044] to-[#6b4f2a] p-8 text-white shadow-xl">
            <p className="mb-3 text-sm font-semibold uppercase tracking-wide text-[#e2c891]">
              Hotel Lakeview
            </p>

            <h1 className="max-w-3xl text-4xl font-bold md:text-5xl">
              Hotellin varaukset, huoneet ja asiakashallinta yhdessä paikassa.
            </h1>

            <p className="mt-5 max-w-2xl text-slate-200">
              Vastaanoton työkalu, jolla voidaan hakea vapaat huoneet, luoda
              varauksia ja hallita hotellin ydintoimintoja selaimessa.
            </p>
          </section>

          <section className="mb-8">
            <h2 className="mb-4 text-2xl font-bold text-[#162033]">
              Pikatoiminnot
            </h2>

            <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
              {quickActions.map((action) => (
                <Link
                  key={action.href}
                  href={action.href}
                  className="rounded-2xl border border-[#8a6a3f]/30 bg-gradient-to-br from-[#243044] via-[#334057] to-[#6b4f2a] p-5 text-white shadow-md transition hover:-translate-y-1 hover:shadow-xl"
                >
                  <h3 className="text-lg font-bold text-[#fff4df]">
                    {action.title}
                  </h3>

                  <p className="mt-2 text-sm text-slate-200">
                    {action.description}
                  </p>
                </Link>
              ))}
            </div>
          </section>

          <section className="grid gap-4 md:grid-cols-3">
            <div className="rounded-2xl border border-[#8a6a3f]/30 bg-gradient-to-br from-[#243044] via-[#334057] to-[#6b4f2a] p-6 text-white shadow-md">
              <p className="text-sm font-medium text-[#e2c891]">
                Varausten hallinta
              </p>
              <p className="mt-2 text-2xl font-bold text-[#fff4df]">
                Päällekkäisyydet estetty
              </p>
            </div>

            <div className="rounded-2xl border border-[#8a6a3f]/30 bg-gradient-to-br from-[#243044] via-[#334057] to-[#6b4f2a] p-6 text-white shadow-md">
              <p className="text-sm font-medium text-[#e2c891]">Huonekuvat</p>
              <p className="mt-2 text-2xl font-bold text-[#fff4df]">
                Azure Blob Storage
              </p>
            </div>

            <div className="rounded-2xl border border-[#8a6a3f]/30 bg-gradient-to-br from-[#243044] via-[#334057] to-[#6b4f2a] p-6 text-white shadow-md">
              <p className="text-sm font-medium text-[#e2c891]">
                Käyttöoikeudet
              </p>
              <p className="mt-2 text-2xl font-bold text-[#fff4df]">
                JWT + roolit
              </p>
            </div>
          </section>
        </div>
      </main>
    </AuthGuard>
  );
}