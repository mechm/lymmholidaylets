import { Metadata } from 'next';
import ReviewCard, { Review } from '@/components/reviews/ReviewCard';

// Force dynamic rendering to ensure fresh data
export const dynamic = 'force-dynamic';

export const metadata: Metadata = {
    title: 'Customer Reviews | Lymm Holiday Lets',
    description: 'Read what our guests have to say about their stay at Lymm Holiday Lets.',
};

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || process.env.API_BASE_URL || 'http://localhost:8080';
const FULL_API_URL = `${API_BASE_URL}/api/review/init`;

async function getReviews(): Promise<Review[]> {
    try {
        const response = await fetch(FULL_API_URL, {
            cache: 'no-store',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        if (!response.ok) {
            console.error(`Failed to fetch reviews: ${response.status} ${response.statusText}`);
            // Return empty array instead of null to simplify rendering logic, or throw to trigger error boundary
            return [];
        }

        const data = await response.json();
        return Array.isArray(data) ? data : [];
    } catch (error) {
        console.error('Error fetching reviews:', error);
        return [];
    }
}

export default async function ReviewsPage() {
    const reviews = await getReviews();

    return (
        <section className="reviews primary-bg empty-space">
            <div className="container">
                <div className="title-box text-center mb-10">
                    <h2 className="title-text white-color">Reviews</h2>
                </div>

                <div className="row">
                    <div className="col-md-9 mx-auto">
                        <div className="reviews-grid grid gap-6 md:grid-cols-2 lg:grid-cols-3">
                            {reviews.length > 0 ? (
                                reviews.map((review, index) => (
                                    <ReviewCard
                                        key={`${review.name}-${index}`}
                                        review={review}
                                    />
                                ))
                            ) : (
                                <div className="text-center py-10 col-span-full">
                                    <p className="white-color text-lg opacity-80">
                                        No reviews available at the moment.
                                    </p>
                                </div>
                            )}
                        </div>
                    </div>
                </div>
            </div>
        </section>
    );
}
