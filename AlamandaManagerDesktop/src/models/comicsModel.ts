export interface ComicModel {
  categoriesIds: number[];
  color?: number | null;
  cover?: number | null;
  ownerId?: number | null;
  status?: number | null;
  available_Storage?: number | null;
  total_Pages?: number | null;
  publish_Date?: string | Date | null;
}

export interface ChaptersListModel {
  items: ChapterModel[];
  totalPages: number;
  currentPage: number;
  comic: ComicModel;
}

export interface ChapterFormModel {
  id?: number;
  images?: (string | null)[];
  file?: string | null;
  comicId: number;
  original_Language: number;
  translations: ChapterTranslationModel[];
}

export interface ChapterMoveModel {
  id?: number;
  position?: number;
}

export interface PageModel {
  id?: number;
  chapterId?: number;
  picture?: string | null;
  position?: number;
  chapter?: ChapterModel | null;
}

export interface ChapterTranslationModel {
  id?: number;
  name: string;
  description?: string | null;
  chapterId?: number;
  languageId?: number;
}

export interface ChapterModel {
  id?: number;
  publish_Date?: string;
  position?: number;
  status?: number;
  original_Language: number;
  comicId: number;
  comic?: ComicModel;
  translations: ChapterTranslationModel[];
  pages: PageModel[];
}

