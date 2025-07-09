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
  comicChapters: (comicId: number | string) => `/comics/${comicId}/chapters`
}

export default urls;