import Image from 'next/image';

export interface Review {
    name: string;
    description: string;
    position?: string;
    company?: string;
    dateTimeAdded: string;
}

interface ReviewCardProps {
    review: Review;
}

export default function ReviewCard({ review }: ReviewCardProps) {
    const formattedDate = new Date(review.dateTimeAdded).toLocaleDateString('en-GB', {
        year: 'numeric',
        month: 'short'
    });

    const positionDetails = [review.position, review.company]
        .filter(Boolean)
        .join(' at ');

    return (
        <div className="reviews-box h-full flex flex-col justify-between">
            <div className="reviews-info text-center flex-grow">
                <div className="quote-icon mb-4 flex justify-center">
                    <Image
                        src="/images/quotes.png"
                        alt=""
                        width={30}
                        height={30}
                        aria-hidden="true"
                    />
                </div>
                <blockquote className="white-color italic">"{review.description}"</blockquote>
            </div>
            <div className="reviews-details text-center mt-4">
                <div className="reviews-name">
                    <p className="font-bold text-white">{review.name}</p>
                    {positionDetails && (
                        <p className="text-sm opacity-75 text-white">
                            {positionDetails}
                        </p>
                    )}
                    <p className="text-xs opacity-50 text-white mt-1">
                        {formattedDate}
                    </p>
                </div>
            </div>
        </div>
    );
}
