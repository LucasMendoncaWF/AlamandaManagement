const urls = {
  login: '/',
  home: '/home',
  comics: '/comics',
  arts: '/arts',
  team: '/team',
  users: '/users',
  roles: '/roles',
  categories: '/categories',
  chapters: '/chapters',
  comicChapters: (comicId: number | string) => `/chapters/${comicId}`
}

export default urls;