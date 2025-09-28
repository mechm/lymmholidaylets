import { Key, ReactElement, JSXElementConstructor, ReactNode, ReactPortal } from "react"

// Tells Next.js to render this page dynamically on every request (SSR behavior)
// This is the simplest way to prevent fetching at build time.
export const dynamic = 'force-dynamic';

// The URL uses the Docker service name, as required by Docker networking
const API_URL = 'http://lymmholidaylets-api:80/weatherforecast';

async function getWeatherData() {
    try {
        // This fetch runs *at runtime* when a user requests the page
        const response = await fetch(API_URL, {
            // Ensure no caching is performed, reinforcing the dynamic behavior
            cache: 'no-store',
        });

        if (!response.ok) {
            // This will handle the case where the API is down during a request
            throw new Error(`API call failed with status: ${response.status}`);
        }

        return response.json();
    } catch (error) {
        console.error('Failed to fetch weather data:', error);
        return null; // Handle failure gracefully
    }
}



export default async function Page() {
    const data = await getWeatherData();

    return (
        <main>
            <h1>Lymm Holiday Lets - Weather Forecast</h1>
            {data ? (
                <ul>
                    {data.map((item: { date: string; summary: string; temperatureC: string; }) => (
                        <li>
                            {item.date} - {item.summary} ({item.temperatureC}�C)
                        </li>
                    ))}
                </ul>
            ) : (
                <p>Loading weather data failed. Please try again later.</p>
            )}
        </main>
    );

  ////return (<>Hello world</>)
  //  //const data = await fetch('http://localhost:8080/weatherforecast')
  //  const data = await fetch('http://lymmholidaylets-api-1:8080/weatherforecast');
  // const posts = await data.json()
  // return (
  //     <ul>
  //     {
  //       posts.map((post: { date: string;}) => (
  //         <li key={post.date}>{post.date}</li>
  //       ))
  //     }
  //   </ul>
  // )
}