
// export default async function Page() {
//
//
//     return (
//     <h1>Reviews</h1>
//     );
// }

interface PropertyDetailPageProps {
  // 'params' will contain the dynamic segments from the URL
  params: {
    id: string; // The value of [id] is always a string
  };
}

// 1. Apply the interface to the function signature
export default async function Page({ params }: PropertyDetailPageProps) {

  // 2. Access 'id' directly from the 'params' object (no 'await' needed)
  const { id } = params
  // Now 'id' is correctly accessed and strongly typed as a string.

  return (
    <main>
      <h1>Lymm Holiday Lets - Weather Forecast</h1>
      <p>Viewing Property ID: <span className="font-mono">{id}</span></p>
    </main>
  );
}