import { createFileRoute } from "@tanstack/react-router";
import ShelfWiseApp from "@/shelfwise/App";

export const Route = createFileRoute("/")({
  head: () => ({
    meta: [
      { title: "ShelfWise - Library Management" },
      { name: "description", content: "Search, circulate, and curate your library catalog with an AI Librarian." },
      { property: "og:title", content: "ShelfWise - Library Management" },
      { property: "og:description", content: "Search, circulate, and curate your library catalog with an AI Librarian." },
    ],
  }),
  component: Index,
});

function Index() {
  return (
    <div className="shelfwise-app">
      <ShelfWiseApp />
    </div>
  );
}
