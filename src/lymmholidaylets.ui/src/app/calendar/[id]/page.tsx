import { Key, ReactElement, JSXElementConstructor, ReactNode, ReactPortal } from "react"

// Tells Next.js to render this page dynamically on every request (SSR behavior)
// This is the simplest way to prevent fetching at build time.
export const dynamic = 'force-dynamic';

// ----------------------------------------------------------------------
// UPDATED: Use an environment variable with a safe local fallback.
// Fallback port (8080) should match your local .NET API port when not in Docker.
//const API_BASE_URL = 'http://localhost:5026';
const API_BASE_URL = process.env.API_BASE_URL || 'http://localhost:8080';
const FULL_API_URL = `${API_BASE_URL}/api/calendar/ical/1.ics?s=D93DA743-F66D-4D8E-97E1-6F94D81E0ABE`;


// ----------------------------------------------------------------------

async function getWeatherData() {
    try {
        // This fetch runs *at runtime* when a user requests the page
        // const response = await fetch(FULL_API_URL, {
        //     // Ensure no caching is performed, reinforcing the dynamic behavior
        //     cache: 'no-store',
        // });

        // 1. Fetch the iCal data from the external API
        const response = await fetch(FULL_API_URL, {
            headers: {
                'Accept': 'text/calendar',
            },
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
}