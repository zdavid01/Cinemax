export interface Movie {
    id: string | number;
    title: string;
    length?: number;
    genre?: string;
    director?: string;
    actors?: string;
    description?: string;
    imageUrl: string;
    trailerLink?: string;
    rating: string | number;
    price?: number;
}
