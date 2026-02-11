import { Key, ReactElement, JSXElementConstructor, ReactNode, ReactPortal } from "react"

import  Slideshow from "../components/homepage/Slideshow";
import WhyChooseUs from "@/components/homepage/WhyChooseUs";

// Tells Next.js to render this page dynamically on every request (SSR behavior)
// This is the simplest way to prevent fetching at build time.
export const dynamic = 'force-dynamic';

// ----------------------------------------------------------------------
// UPDATED: Use an environment variable with a safe local fallback.
// Fallback port (8080) should match your local .NET API port when not in Docker.
const API_BASE_URL = process.env.API_BASE_URL || 'http://localhost:8080';
const FULL_API_URL = `${API_BASE_URL}/weatherforecast`;
// ----------------------------------------------------------------------

async function getWeatherData() {
    try {
        // This fetch runs *at runtime* when a user requests the page
        const response = await fetch(FULL_API_URL, {
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

// Define the shape of a single slide
interface Slide {
  url: string;
  title?: string;
}

const sliderData: Slide[] = [
  { url: 'https://images.unsplash.com/photo-1464822759023-fed622ff2c3b', title: 'Mountains' },
  { url: 'https://images.unsplash.com/photo-1501785888041-af3ef285b470', title: 'Lake' },
];

               const mySlides = [
  { 
    imagePath: "https://images.unsplash.com/photo-1464822759023-fed622ff2c3b", 
    imagePathAlt: "Cruise in Caribbean", 
    captionTitle: "Tropical Escape", 
    caption: "Book your winter sun now.", 
    link: "/book/caribbean" 
  },
    { 
    imagePath: "https://images.unsplash.com/photo-1501785888041-af3ef285b470", 
    imagePathAlt: "Cruise in Caribbean", 
    captionTitle: "Tropical Escape", 
    caption: "Book your winter sun now.", 
    link: "/book/caribbean" 
  }
];

export default async function Page() {
    const data = await getWeatherData();

    return (
        <>
            {/* <h1>Lymm Holiday Lets - Weather Forecast</h1>
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
            )} */}

{/* 
               <Slideshow slides={sliderData} /> */}



<Slideshow slides={mySlides} />
<WhyChooseUs />
        </>





    );
}