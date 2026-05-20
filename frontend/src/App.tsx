import { useEffect, useMemo, useState } from 'react'
import type { FormEvent, ReactNode } from 'react'
import {
  ArrowLeft,
  BookOpen,
  ChevronDown,
  ChevronUp,
  Copy,
  Dices,
  Download,
  Edit,
  Heart,
  Home,
  IdCard,
  Library,
  LogIn,
  LogOut,
  Minus,
  Moon,
  Plus,
  RefreshCw,
  ScrollText,
  ShieldCheck,
  Sparkles,
  Sun,
  Swords,
  Trash2,
  UserPlus,
  Users,
} from 'lucide-react'
import {
  BrowserRouter,
  Link,
  Navigate,
  NavLink,
  Outlet,
  Route,
  Routes,
  useLocation,
  useNavigate,
  useParams,
} from 'react-router-dom'

type AuthMode = 'login' | 'register'
type Theme = 'light' | 'dark'
type UserProfile = 'Player' | 'GameMaster'

type UserResponse = {
  id: string
  name: string
  email: string
  profile: UserProfile
  avatarUrl?: string | null
  createdAt: string
}

type AuthResponse = {
  token: string
  user: UserResponse
}

type CampaignRole = 'Player' | 'Master'

type CampaignSummaryResponse = {
  id: string
  name: string
  description: string
  system: string
  coverImageUrl?: string | null
  createdAt: string
  currentUserRole: CampaignRole
  memberCount: number
}

type CampaignMemberResponse = {
  id: string
  userId: string
  userName: string
  email: string
  role: CampaignRole
  joinedAt: string
}

type CampaignResponse = {
  id: string
  name: string
  description: string
  system: string
  coverImageUrl?: string | null
  inviteCode: string
  createdByUserId: string
  createdAt: string
  updatedAt?: string | null
  currentUserRole: CampaignRole
  members: CampaignMemberResponse[]
}

type CampaignCharacterSummaryResponse = {
  id: string
  userId: string
  userName: string
  name: string
  mainClass: string
  totalLevel: number
  currentHitPoints: number
  maxHitPoints: number
  armorClass: number
  passivePerception: number
}

type CampaignMasterNoteResponse = {
  id: string
  characterId: string
  characterName: string
  title: string
  content: string
  category: string
  tags: string
  createdAt: string
  updatedAt?: string | null
}

type CampaignMasterDashboardResponse = {
  campaignId: string
  campaignName: string
  members: CampaignMemberResponse[]
  characters: CampaignCharacterSummaryResponse[]
  visibleNotes: CampaignMasterNoteResponse[]
}

type CampaignPayload = {
  name: string
  description: string
  system: string
  coverImageUrl?: string | null
}

type CharacterSummaryResponse = {
  id: string
  userId: string
  campaignId?: string | null
  campaignName?: string | null
  name: string
  nickname?: string | null
  avatarUrl?: string | null
  totalLevel: number
  species: string
  mainClass: string
  subclass: string
  armorClass: number
  currentHitPoints: number
  maxHitPoints: number
  canEdit: boolean
}

type CharacterResponse = CharacterPayload & {
  id: string
  userId: string
  campaignName?: string | null
  createdAt: string
  updatedAt?: string | null
  canEdit: boolean
}

type CharacterPayload = {
  campaignId?: string | null
  name: string
  nickname?: string | null
  avatarUrl?: string | null
  tokenImageUrl?: string | null
  totalLevel: number
  species: string
  mainClass: string
  subclass: string
  background: string
  alignment: string
  experience: number
  inspiration: boolean
  proficiencyBonus: number
  armorClass: number
  initiative: number
  speed: number
  maxHitPoints: number
  currentHitPoints: number
  temporaryHitPoints: number
  totalHitDice: string
  availableHitDice: string
  physicalDescription: string
  personalityTraits: string
  ideals: string
  bonds: string
  flaws: string
  backstory: string
  quickNotes: string
}

type AbilityType =
  | 'Strength'
  | 'Dexterity'
  | 'Constitution'
  | 'Intelligence'
  | 'Wisdom'
  | 'Charisma'

type AbilityScoreResponse = {
  attribute: AbilityType
  label: string
  score: number
  modifier: number
}

type SavingThrowResponse = {
  attribute: AbilityType
  label: string
  modifier: number
  isProficient: boolean
  customBonus: number
  finalValue: number
}

type SkillType =
  | 'Acrobatics'
  | 'AnimalHandling'
  | 'Arcana'
  | 'Athletics'
  | 'Deception'
  | 'History'
  | 'Insight'
  | 'Intimidation'
  | 'Investigation'
  | 'Medicine'
  | 'Nature'
  | 'Perception'
  | 'Performance'
  | 'Persuasion'
  | 'Religion'
  | 'SleightOfHand'
  | 'Stealth'
  | 'Survival'

type CharacterSkillResponse = {
  id: string
  skillType: SkillType
  label: string
  baseAttribute: AbilityType
  baseAttributeLabel: string
  isProficient: boolean
  isExpertise: boolean
  customBonus: number
  finalValue: number
}

type CharacterCombatResponse = {
  armorClass: number
  initiative: number
  speed: number
  maxHitPoints: number
  currentHitPoints: number
  temporaryHitPoints: number
  totalHitDice: string
  availableHitDice: string
}

type CharacterAttackResponse = {
  id: string
  name: string
  attackBonus: number
  damage: string
  damageType: string
  range: string
  usesAttribute?: AbilityType | null
  usesAttributeLabel?: string | null
  notes: string
}

type ConditionType =
  | 'Blinded'
  | 'Charmed'
  | 'Deafened'
  | 'Frightened'
  | 'Grappled'
  | 'Incapacitated'
  | 'Invisible'
  | 'Paralyzed'
  | 'Petrified'
  | 'Poisoned'
  | 'Prone'
  | 'Restrained'
  | 'Stunned'
  | 'Unconscious'
  | 'Exhaustion'

type CharacterConditionResponse = {
  id: string
  conditionType: ConditionType
  name: string
  description: string
  isActive: boolean
  notes: string
}

type CharacterNoteResponse = {
  id: string
  characterId: string
  title: string
  content: string
  category: string
  tags: string
  isPrivate: boolean
  isVisibleToMaster: boolean
  createdAt: string
  updatedAt?: string | null
  canEdit: boolean
}

type ItemType = 'Weapon' | 'Armor' | 'Consumable' | 'Tool' | 'MagicItem' | 'Treasure' | 'Other'

type CharacterInventoryItemResponse = {
  id: string
  characterId: string
  name: string
  description: string
  quantity: number
  weight: number
  value: number
  itemType: ItemType
  itemTypeLabel: string
  equipped: boolean
  attuned: boolean
  notes: string
  totalWeight: number
  canEdit: boolean
}

type CharacterCurrencyResponse = {
  copper: number
  silver: number
  electrum: number
  gold: number
  platinum: number
  canEdit: boolean
}

type AssetType = 'Avatar' | 'Token' | 'Gallery' | 'Document'

type CharacterAssetResponse = {
  id: string
  characterId: string
  fileName: string
  fileUrl: string
  fileType: string
  assetType: AssetType
  uploadedAt: string
  canEdit: boolean
}

type CharacterSpellResponse = {
  id: string
  characterId: string
  spellId: string
  name: string
  englishName: string
  level: number
  school: string
  castingTime: string
  range: string
  components: string
  material: string
  duration: string
  isConcentration: boolean
  isRitual: boolean
  description: string
  higherLevelDescription: string
  availableClasses: string
  source: string
  isHomebrew: boolean
  isKnown: boolean
  isPrepared: boolean
  isFavorite: boolean
  notes: string
  canEdit: boolean
}

type CharacterSpellSlotResponse = {
  id: string
  characterId: string
  spellLevel: number
  totalSlots: number
  usedSlots: number
  canEdit: boolean
}

type SpellVisibility = 'Private' | 'Campaign' | 'LocalPublic'

type SpellPayload = {
  name: string
  englishName: string
  level: number
  school: string
  castingTime: string
  range: string
  components: string
  material: string
  duration: string
  isConcentration: boolean
  isRitual: boolean
  description: string
  higherLevelDescription: string
  availableClasses: string
  source: string
  isHomebrew: boolean
  visibility: SpellVisibility
  campaignId?: string | null
}

type SpellResponse = SpellPayload & {
  id: string
  createdByUserId: string
  createdByUserName: string
  campaignName?: string | null
  createdAt: string
  updatedAt?: string | null
  canEdit: boolean
}

type PagedResponse<T> = {
  items: T[]
  page: number
  pageSize: number
  totalItems: number
  totalPages: number
}

type FeatureType = 'Feat' | 'Class' | 'Subclass' | 'Species' | 'Background' | 'Homebrew'

type FeaturePayload = {
  name: string
  type: FeatureType
  description: string
  source: string
  prerequisites: string
  isHomebrew: boolean
  visibility: SpellVisibility
  campaignId?: string | null
}

type FeatureResponse = FeaturePayload & {
  id: string
  createdByUserId: string
  createdByUserName: string
  campaignName?: string | null
  createdAt: string
  updatedAt?: string | null
  canEdit: boolean
}

type RecoveryType = 'Manual' | 'ShortRest' | 'LongRest'

type CharacterFeaturePayload = {
  featureId?: string | null
  customName: string
  customDescription: string
  maxUses: number
  currentUses: number
  recoveryType: RecoveryType
  notes: string
}

type CharacterFeatureResponse = CharacterFeaturePayload & {
  id: string
  characterId: string
  name: string
  type?: FeatureType | null
  typeLabel?: string | null
  description: string
  source: string
  prerequisites: string
  isHomebrew: boolean
  recoveryTypeLabel: string
  canEdit: boolean
}

type DiceRollRequest = {
  expression: string
  advantage: boolean
  disadvantage: boolean
  label?: string | null
}

type DiceRollResponse = {
  expression: string
  rolls: number[]
  modifier: number
  total: number
  label?: string | null
  rolledAt: string
}

type RollDiceFn = (request: DiceRollRequest) => Promise<DiceRollResponse | null>

type RestType = 'short' | 'long'

type CharacterRestRequest = {
  restoreHitPoints: boolean
  restoreHitDice: boolean
}

type CharacterRestResponse = {
  character: CharacterResponse
  features: CharacterFeatureResponse[]
  spellSlots: CharacterSpellSlotResponse[]
}

type CharacterPdfData = {
  character: CharacterResponse
  playerName: string
  abilities: AbilityScoreResponse[]
  skills: CharacterSkillResponse[]
  combat: CharacterCombatResponse | null
  attacks: CharacterAttackResponse[]
  spells: CharacterSpellResponse[]
  features: CharacterFeatureResponse[]
  inventory: CharacterInventoryItemResponse[]
}

type AuthContextValue = {
  token: string
  user: UserResponse | null
  isBooting: boolean
  login: (auth: AuthResponse) => void
  logout: () => void
}

const DEFAULT_API_BASE_URL = import.meta.env.PROD
  ? 'https://rpgmanagerapp-btd7afa0htdde0df.brazilsouth-01.azurewebsites.net/api'
  : 'http://localhost:5000/api'
const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL ?? DEFAULT_API_BASE_URL).replace(/\/$/, '')
const API_ORIGIN = new URL(API_BASE_URL).origin
const TOKEN_KEY = 'rpgmanager.token'
const THEME_KEY = 'rpgmanager.theme'

const navigation = [
  { label: 'Dashboard', path: '/dashboard', icon: Home },
  { label: 'Campanhas', path: '/campaigns', icon: Users },
  { label: 'Personagens', path: '/characters', icon: Swords },
  { label: 'Magias', path: '/spells', icon: Library },
  { label: 'Talentos', path: '/features', icon: Sparkles },
]

const baseVisibilityOptions: [string, string][] = [
  ['Private', 'Privada'],
  ['LocalPublic', 'Pública local'],
]

function contentVisibilityOptions(canCreateCampaignContent: boolean): [string, string][] {
  return canCreateCampaignContent
    ? [['Private', 'Privada'], ['Campaign', 'Campanha'], ['LocalPublic', 'Pública local']]
    : baseVisibilityOptions
}

function apiUrl(path: string) {
  const normalizedPath = path.startsWith('/api/') ? path.slice(4) : path
  return `${API_BASE_URL}${normalizedPath.startsWith('/') ? normalizedPath : `/${normalizedPath}`}`
}

async function apiRequest<T>(path: string, token: string, options: RequestInit = {}): Promise<T> {
  const response = await fetch(apiUrl(path), {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
      ...options.headers,
    },
  })

  const text = await response.text()
  const data = text ? JSON.parse(text) : null

  if (!response.ok) {
    throw new Error(data?.message ?? 'Operação não concluída.')
  }

  return data as T
}

async function safeApiRequest<T>(path: string, token: string, fallback: T): Promise<T> {
  try {
    return await apiRequest<T>(path, token)
  } catch {
    return fallback
  }
}

async function apiFormRequest<T>(path: string, token: string, formData: FormData, method = 'POST'): Promise<T> {
  const response = await fetch(apiUrl(path), {
    method,
    headers: {
      Authorization: `Bearer ${token}`,
    },
    body: formData,
  })

  const text = await response.text()
  const data = text ? JSON.parse(text) : null

  if (!response.ok) {
    throw new Error(data?.message ?? 'Operação não concluída.')
  }

  return data as T
}

function App() {
  const [token, setToken] = useState(() => localStorage.getItem(TOKEN_KEY) ?? '')
  const [user, setUser] = useState<UserResponse | null>(null)
  const [isBooting, setIsBooting] = useState(Boolean(token))
  const [theme, setTheme] = useState<Theme>(() => (localStorage.getItem(THEME_KEY) as Theme) || 'light')

  useEffect(() => {
    document.documentElement.classList.toggle('dark', theme === 'dark')
    document
      .querySelector('meta[name="theme-color"]')
      ?.setAttribute('content', theme === 'dark' ? '#020617' : '#f8fafc')
    localStorage.setItem(THEME_KEY, theme)
  }, [theme])

  useEffect(() => {
    if (!token) {
      return
    }

    fetch(apiUrl('/api/auth/me'), {
      headers: { Authorization: `Bearer ${token}` },
    })
      .then(async (response) => {
        if (!response.ok) {
          throw new Error('Sessão expirada.')
        }
        setUser(await response.json())
      })
      .catch(() => {
        localStorage.removeItem(TOKEN_KEY)
        setToken('')
        setUser(null)
      })
      .finally(() => setIsBooting(false))
  }, [token])

  const auth = useMemo<AuthContextValue>(
    () => ({
      token,
      user,
      isBooting,
      login: (authResponse) => {
        localStorage.setItem(TOKEN_KEY, authResponse.token)
        setToken(authResponse.token)
        setUser(authResponse.user)
      },
      logout: () => {
        localStorage.removeItem(TOKEN_KEY)
        setToken('')
        setUser(null)
      },
    }),
    [isBooting, token, user],
  )

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage auth={auth} />} />
        <Route
          path="/"
          element={
            <ProtectedRoute auth={auth}>
              <AppLayout auth={auth} setTheme={setTheme} theme={theme} />
            </ProtectedRoute>
          }
        >
          <Route index element={<Navigate replace to="/dashboard" />} />
          <Route path="dashboard" element={<Dashboard user={user} />} />
          <Route path="campaigns" element={<CampaignsPage auth={auth} />} />
          <Route path="campaigns/new" element={<CampaignFormPage auth={auth} />} />
          <Route path="campaigns/:id" element={<CampaignDetailPage auth={auth} />} />
          <Route path="campaigns/:id/edit" element={<CampaignFormPage auth={auth} />} />
          <Route path="characters" element={<CharactersPage auth={auth} />} />
          <Route path="characters/new" element={<CharacterFormPage auth={auth} />} />
          <Route path="characters/:id" element={<CharacterDetailPage auth={auth} />} />
          <Route path="characters/:id/edit" element={<CharacterFormPage auth={auth} />} />
          <Route path="spells" element={<SpellsPage auth={auth} />} />
          <Route path="features" element={<FeaturesPage auth={auth} />} />
          <Route path="homebrew" element={<Navigate replace to="/features" />} />
        </Route>
        <Route path="*" element={<Navigate replace to={user ? '/dashboard' : '/login'} />} />
      </Routes>
    </BrowserRouter>
  )
}

function ProtectedRoute({ auth, children }: { auth: AuthContextValue; children: ReactNode }) {
  const location = useLocation()

  if (auth.isBooting) {
    return <LoadingScreen />
  }

  if (!auth.user) {
    return <Navigate replace state={{ from: location }} to="/login" />
  }

  return children
}

function LoginPage({ auth }: { auth: AuthContextValue }) {
  const [mode, setMode] = useState<AuthMode>('login')
  const [name, setName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [profile, setProfile] = useState<UserProfile>('Player')
  const [message, setMessage] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const navigate = useNavigate()
  const location = useLocation()

  const from = (location.state as { from?: { pathname?: string } } | null)?.from?.pathname ?? '/dashboard'

  useEffect(() => {
    if (auth.user) {
      navigate('/dashboard', { replace: true })
    }
  }, [auth.user, navigate])

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setIsLoading(true)
    setMessage('')

    const payload = mode === 'register' ? { name, email, password, profile } : { email, password }

    try {
      const response = await fetch(apiUrl(`/api/auth/${mode}`), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
      })

      const data = await response.json()
      if (!response.ok) {
        throw new Error(data.message ?? 'Não foi possível autenticar.')
      }

      auth.login(data as AuthResponse)
      setPassword('')
      navigate(from, { replace: true })
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro inesperado.')
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <main className="min-h-screen bg-slate-100 text-slate-950 dark:bg-slate-950 dark:text-slate-100">
      <section className="mx-auto grid min-h-screen w-full max-w-6xl items-center gap-8 px-4 py-8 sm:px-6 lg:grid-cols-[1fr_420px] lg:px-8">
        <div className="max-w-2xl">
          <div className="mb-8 flex items-center gap-3">
            <span className="flex size-11 items-center justify-center rounded-lg bg-emerald-600 text-white">
              <ShieldCheck size={24} />
            </span>
            <div>
              <h1 className="text-xl font-semibold">RPG Manager</h1>
              <p className="text-sm text-slate-500 dark:text-slate-400">Ficha digital privada</p>
            </div>
          </div>
          <p className="mb-3 text-sm font-semibold uppercase text-emerald-700 dark:text-emerald-400">
            MVP autenticado
          </p>
          <h2 className="text-4xl font-semibold tracking-tight sm:text-5xl">
            Organize mesa, personagens e conteúdo no mesmo lugar.
          </h2>
          <p className="mt-5 text-base leading-7 text-slate-600 dark:text-slate-300">
            Núcleo pronto para campanhas, fichas e bibliotecas. Login protege dashboard e futuras telas.
          </p>
        </div>

        <form
          className="rounded-lg border border-slate-200 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900"
          onSubmit={handleSubmit}
        >
          <div className="mb-5 grid grid-cols-2 rounded-md bg-slate-100 p-1 dark:bg-slate-800">
            <button
              className={`rounded px-3 py-2 text-sm font-medium ${mode === 'login' ? 'bg-white text-slate-950 shadow-sm dark:bg-slate-950 dark:text-white' : 'text-slate-600 dark:text-slate-300'}`}
              onClick={() => setMode('login')}
              type="button"
            >
              Entrar
            </button>
            <button
              className={`rounded px-3 py-2 text-sm font-medium ${mode === 'register' ? 'bg-white text-slate-950 shadow-sm dark:bg-slate-950 dark:text-white' : 'text-slate-600 dark:text-slate-300'}`}
              onClick={() => setMode('register')}
              type="button"
            >
              Criar conta
            </button>
          </div>

          <div className="space-y-4">
            {mode === 'register' && (
              <>
                <TextField label="Nome" onChange={setName} required value={name} />
                <label className="block">
                  <span className="text-sm font-medium text-slate-700 dark:text-slate-200">Perfil inicial</span>
                  <select
                    className="mt-1 min-h-11 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
                    onChange={(event) => setProfile(event.target.value as UserProfile)}
                    value={profile}
                  >
                    <option value="Player">Jogador</option>
                    <option value="GameMaster">Mestre</option>
                  </select>
                </label>
              </>
            )}

            <TextField label="E-mail" onChange={setEmail} required type="email" value={email} />
            <TextField label="Senha" minLength={8} onChange={setPassword} required type="password" value={password} />

            {message && <p className="rounded-md bg-red-50 p-3 text-sm text-red-700 dark:bg-red-950 dark:text-red-200">{message}</p>}

            <button
              className="flex w-full items-center justify-center gap-2 rounded-md bg-emerald-600 px-4 py-3 font-semibold text-white disabled:cursor-not-allowed disabled:bg-slate-400"
              disabled={isLoading}
              type="submit"
            >
              {mode === 'login' ? <LogIn size={18} /> : <UserPlus size={18} />}
              {isLoading ? 'Enviando...' : mode === 'login' ? 'Entrar' : 'Cadastrar'}
            </button>
          </div>
        </form>
      </section>
    </main>
  )
}

function TextField({
  label,
  minLength,
  onChange,
  required,
  type = 'text',
  value,
}: {
  label: string
  minLength?: number
  onChange: (value: string) => void
  required?: boolean
  type?: string
  value: string
}) {
  return (
    <label className="block">
      <span className="text-sm font-medium text-slate-700 dark:text-slate-200">{label}</span>
      <input
        className="mt-1 min-h-11 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition placeholder:text-slate-400 focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
        minLength={minLength}
        onChange={(event) => onChange(event.target.value)}
        required={required}
        type={type}
        value={value}
      />
    </label>
  )
}

function AppLayout({
  auth,
  setTheme,
  theme,
}: {
  auth: AuthContextValue
  setTheme: (theme: Theme) => void
  theme: Theme
}) {
  const navigate = useNavigate()

  function logout() {
    auth.logout()
    navigate('/login', { replace: true })
  }

  return (
    <div className="min-h-screen bg-slate-100 text-slate-950 dark:bg-slate-950 dark:text-slate-100">
      <Sidebar />
      <div className="min-h-screen lg:pl-72">
        <header className="sticky top-0 z-20 border-b border-slate-200 bg-white/95 px-4 py-3 shadow-sm backdrop-blur dark:border-slate-800 dark:bg-slate-950/90 sm:px-6 lg:shadow-none">
          <div className="flex items-center justify-between gap-3">
            <div className="flex min-w-0 items-center gap-3">
              <span className="flex size-10 items-center justify-center rounded-lg bg-emerald-600 text-white lg:hidden">
                <ShieldCheck size={21} />
              </span>
              <div className="min-w-0">
                <p className="truncate text-sm text-slate-500 dark:text-slate-400">Sessão ativa</p>
                <h1 className="truncate text-lg font-semibold">{auth.user?.name}</h1>
              </div>
            </div>
            <div className="flex items-center gap-2">
              <button
                aria-label="Alternar tema"
                className="flex size-11 items-center justify-center rounded-md border border-slate-300 bg-white text-slate-700 transition hover:border-emerald-400 hover:text-emerald-700 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-200 dark:hover:border-emerald-700"
                onClick={() => setTheme(theme === 'dark' ? 'light' : 'dark')}
                type="button"
              >
                {theme === 'dark' ? <Sun size={18} /> : <Moon size={18} />}
              </button>
              <button
                className="flex min-h-11 items-center gap-2 rounded-md border border-slate-300 bg-white px-3 py-2 text-sm font-medium text-slate-700 transition hover:border-emerald-400 hover:text-emerald-700 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-200 dark:hover:border-emerald-700"
                onClick={logout}
                type="button"
              >
                <LogOut size={17} />
                <span className="hidden sm:inline">Sair</span>
              </button>
            </div>
          </div>
        </header>

        <main className="px-4 pb-[calc(6.5rem+env(safe-area-inset-bottom))] pt-5 sm:px-6 sm:pt-6 lg:px-8 lg:pb-10">
          <Outlet />
        </main>
      </div>
      <MobileNavigation />
    </div>
  )
}

function Sidebar() {
  return (
    <aside className="fixed inset-y-0 left-0 hidden w-72 border-r border-slate-200 bg-white px-4 py-5 shadow-sm dark:border-slate-800 dark:bg-slate-950 lg:block">
      <div className="mb-8 flex items-center gap-3 px-2">
        <span className="flex size-11 items-center justify-center rounded-lg bg-emerald-600 text-white">
          <ShieldCheck size={24} />
        </span>
        <div>
          <p className="text-lg font-semibold">RPG Manager</p>
          <p className="text-sm text-slate-500 dark:text-slate-400">Mesa privada</p>
        </div>
      </div>
      <nav className="space-y-1" aria-label="Navegação principal">
        {navigation.map((item) => (
          <NavLink
            className={({ isActive }) =>
              `flex min-h-11 items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition ${
                isActive
                  ? 'bg-emerald-50 text-emerald-700 dark:bg-emerald-950 dark:text-emerald-200'
                  : 'text-slate-600 hover:bg-slate-100 dark:text-slate-300 dark:hover:bg-slate-900'
              }`
            }
            key={item.path}
            to={item.path}
          >
            <item.icon size={18} />
            {item.label}
          </NavLink>
        ))}
      </nav>
    </aside>
  )
}

function MobileNavigation() {
  return (
    <nav
      className="fixed inset-x-0 bottom-0 z-30 grid grid-cols-5 border-t border-slate-200 bg-white/95 px-1 pb-[calc(0.5rem+env(safe-area-inset-bottom))] pt-2 shadow-[0_-10px_30px_rgba(15,23,42,0.08)] backdrop-blur dark:border-slate-800 dark:bg-slate-950/95 lg:hidden"
      aria-label="Navegação principal"
    >
      {navigation.map((item) => (
        <NavLink
          className={({ isActive }) =>
            `flex min-h-14 flex-col items-center justify-center gap-1 rounded-md px-1 py-2 text-[11px] font-semibold transition ${
              isActive
                ? 'bg-emerald-50 text-emerald-700 dark:bg-emerald-950 dark:text-emerald-200'
                : 'text-slate-500 dark:text-slate-400'
            }`
          }
          key={item.path}
          to={item.path}
        >
          <item.icon size={19} />
          <span className="max-w-full truncate">{item.label.split(' ')[0]}</span>
        </NavLink>
      ))}
    </nav>
  )
}

function Dashboard({ user }: { user: UserResponse | null }) {
  const cards = [
    { title: 'Minhas Campanhas', path: '/campaigns', icon: Users, text: 'Mesas criadas, convites e membros.' },
    { title: 'Meus Personagens', path: '/characters', icon: Swords, text: 'Fichas dos seus heróis e NPCs.' },
    { title: 'Biblioteca de Magias', path: '/spells', icon: BookOpen, text: 'Catálogo customizável para magias.' },
    { title: 'Talentos e Características', path: '/features', icon: ScrollText, text: 'Talentos, traços e conteúdo homebrew.' },
  ]

  return (
    <div className="mx-auto max-w-6xl">
      <section className="mb-8">
        <p className="text-sm font-semibold uppercase text-emerald-700 dark:text-emerald-400">Dashboard</p>
        <div className="mt-3 grid gap-4 lg:grid-cols-[1fr_260px] lg:items-end">
          <div>
            <h2 className="text-3xl font-semibold tracking-tight sm:text-4xl">
              Bem-vindo{user?.name ? `, ${user.name.split(' ')[0]}` : ''}.
            </h2>
            <p className="mt-3 max-w-2xl text-slate-600 dark:text-slate-300">
              Base pronta. Próximos módulos entram nestes atalhos sem mudar navegação principal.
            </p>
          </div>
          <div className="rounded-md border border-slate-200 bg-white p-4 text-sm text-slate-700 dark:border-slate-800 dark:bg-slate-900 dark:text-slate-300">
            <p className="font-semibold text-slate-950 dark:text-white">Status MVP</p>
            <p className="mt-1">Autenticação ativa. Layout responsivo ativo.</p>
          </div>
        </div>
      </section>

      <section className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        {cards.map((card) => (
          <NavLink
            className="group rounded-lg border border-slate-200 bg-white p-5 shadow-sm transition hover:-translate-y-0.5 hover:border-emerald-300 hover:shadow-md dark:border-slate-800 dark:bg-slate-900 dark:hover:border-emerald-700"
            key={card.path}
            to={card.path}
          >
            <span className="mb-4 flex size-11 items-center justify-center rounded-md bg-emerald-50 text-emerald-700 dark:bg-emerald-950 dark:text-emerald-300">
              <card.icon size={22} />
            </span>
            <h3 className="text-lg font-semibold">{card.title}</h3>
            <p className="mt-2 text-sm leading-6 text-slate-600 dark:text-slate-300">{card.text}</p>
          </NavLink>
        ))}
      </section>
    </div>
  )
}

function CampaignsPage({ auth }: { auth: AuthContextValue }) {
  const [campaigns, setCampaigns] = useState<CampaignSummaryResponse[]>([])
  const [inviteCode, setInviteCode] = useState('')
  const [message, setMessage] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const [isJoining, setIsJoining] = useState(false)
  const navigate = useNavigate()

  useEffect(() => {
    apiRequest<CampaignSummaryResponse[]>('/api/campaigns', auth.token)
      .then(setCampaigns)
      .catch((error) => setMessage(error instanceof Error ? error.message : 'Erro ao carregar campanhas.'))
      .finally(() => setIsLoading(false))
  }, [auth.token])

  async function joinCampaign(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setIsJoining(true)
    setMessage('')

    try {
      const campaign = await apiRequest<CampaignResponse>('/api/campaigns/join', auth.token, {
        method: 'POST',
        body: JSON.stringify({ inviteCode }),
      })
      setInviteCode('')
      navigate(`/campaigns/${campaign.id}`)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao entrar na campanha.')
    } finally {
      setIsJoining(false)
    }
  }

  return (
    <div className="mx-auto max-w-6xl space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <p className="text-sm font-semibold uppercase text-emerald-700 dark:text-emerald-400">Campanhas</p>
          <h2 className="mt-2 text-3xl font-semibold tracking-tight">Minhas Campanhas</h2>
          <p className="mt-2 max-w-2xl text-slate-600 dark:text-slate-300">
            Crie uma mesa, compartilhe convite, acompanhe membros.
          </p>
        </div>
        <Link
          className="inline-flex items-center justify-center gap-2 rounded-md bg-emerald-600 px-4 py-3 text-sm font-semibold text-white"
          to="/campaigns/new"
        >
          <Plus size={18} />
          Nova campanha
        </Link>
      </div>

      <form
        className="grid gap-3 rounded-lg border border-slate-200 bg-white p-4 dark:border-slate-800 dark:bg-slate-900 sm:grid-cols-[1fr_auto]"
        onSubmit={joinCampaign}
      >
        <label className="block">
          <span className="text-sm font-medium text-slate-700 dark:text-slate-200">Entrar por código</span>
          <input
            className="mt-1 min-h-11 w-full rounded-md border border-slate-300 bg-white px-3 py-2 uppercase text-slate-950 shadow-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
            maxLength={16}
            onChange={(event) => setInviteCode(event.target.value)}
            placeholder="EX: ABCD2345"
            required
            value={inviteCode}
          />
        </label>
        <button
          className="self-end rounded-md border border-slate-300 px-4 py-2.5 text-sm font-semibold text-slate-700 dark:border-slate-700 dark:text-slate-200"
          disabled={isJoining}
          type="submit"
        >
          {isJoining ? 'Entrando...' : 'Entrar'}
        </button>
      </form>

      {message && <p className="rounded-md bg-red-50 p-3 text-sm text-red-700 dark:bg-red-950 dark:text-red-200">{message}</p>}

      {isLoading ? (
        <PanelText>Carregando campanhas...</PanelText>
      ) : campaigns.length === 0 ? (
        <PanelText>Nenhuma campanha ainda. Crie uma mesa ou entre por convite.</PanelText>
      ) : (
        <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          {campaigns.map((campaign) => (
            <Link
              className="rounded-lg border border-slate-200 bg-white p-5 shadow-sm transition hover:-translate-y-0.5 hover:border-emerald-300 hover:shadow-md dark:border-slate-800 dark:bg-slate-900 dark:hover:border-emerald-700"
              key={campaign.id}
              to={`/campaigns/${campaign.id}`}
            >
              <div className="flex items-start justify-between gap-3">
                <div>
                  <h3 className="text-lg font-semibold">{campaign.name}</h3>
                  <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">{campaign.system}</p>
                </div>
                <RoleBadge role={campaign.currentUserRole} />
              </div>
              <p className="mt-4 line-clamp-3 text-sm leading-6 text-slate-600 dark:text-slate-300">
                {campaign.description || 'Sem descrição.'}
              </p>
              <p className="mt-4 text-sm font-medium text-slate-700 dark:text-slate-200">
                {campaign.memberCount} membro{campaign.memberCount === 1 ? '' : 's'}
              </p>
            </Link>
          ))}
        </section>
      )}
    </div>
  )
}

function CampaignFormPage({ auth }: { auth: AuthContextValue }) {
  const { id } = useParams()
  const isEditing = Boolean(id)
  const navigate = useNavigate()
  const [payload, setPayload] = useState<CampaignPayload>({
    name: '',
    description: '',
    system: 'D&D 5e',
    coverImageUrl: '',
  })
  const [message, setMessage] = useState('')
  const [isLoading, setIsLoading] = useState(isEditing)
  const [isSaving, setIsSaving] = useState(false)
  const [canDelete, setCanDelete] = useState(false)
  const [canEdit, setCanEdit] = useState(!isEditing)

  useEffect(() => {
    if (!id) {
      return
    }

    apiRequest<CampaignResponse>(`/api/campaigns/${id}`, auth.token)
      .then((campaign) => {
        setPayload({
          name: campaign.name,
          description: campaign.description,
          system: campaign.system,
          coverImageUrl: campaign.coverImageUrl ?? '',
        })
        setCanDelete(campaign.currentUserRole === 'Master')
        setCanEdit(campaign.currentUserRole === 'Master')
      })
      .catch((error) => setMessage(error instanceof Error ? error.message : 'Erro ao carregar campanha.'))
      .finally(() => setIsLoading(false))
  }, [auth.token, id])

  async function saveCampaign(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setIsSaving(true)
    setMessage('')

    try {
      const campaign = await apiRequest<CampaignResponse>(
        isEditing ? `/api/campaigns/${id}` : '/api/campaigns',
        auth.token,
        {
          method: isEditing ? 'PUT' : 'POST',
          body: JSON.stringify(payload),
        },
      )
      navigate(`/campaigns/${campaign.id}`)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao salvar campanha.')
    } finally {
      setIsSaving(false)
    }
  }

  async function deleteCampaign() {
    if (!id) {
      return
    }

    setIsSaving(true)
    setMessage('')
    try {
      await apiRequest<null>(`/api/campaigns/${id}`, auth.token, { method: 'DELETE' })
      navigate('/campaigns')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao excluir campanha.')
    } finally {
      setIsSaving(false)
    }
  }

  return (
    <div className="mx-auto max-w-3xl space-y-6">
      <BackLink to={id ? `/campaigns/${id}` : '/campaigns'} />
      <div>
        <p className="text-sm font-semibold uppercase text-emerald-700 dark:text-emerald-400">Campanhas</p>
        <h2 className="mt-2 text-3xl font-semibold tracking-tight">
          {isEditing ? 'Editar campanha' : 'Criar campanha'}
        </h2>
      </div>

      {isLoading ? (
        <PanelText>Carregando campanha...</PanelText>
      ) : !canEdit ? (
        <AccessDenied backTo={`/campaigns/${id}`} />
      ) : (
        <form
          className="space-y-4 rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900"
          onSubmit={saveCampaign}
        >
          <TextField
            label="Nome"
            onChange={(value) => setPayload((current) => ({ ...current, name: value }))}
            required
            value={payload.name}
          />
          <TextField
            label="Sistema"
            onChange={(value) => setPayload((current) => ({ ...current, system: value }))}
            required
            value={payload.system}
          />
          <TextAreaField
            label="Descrição"
            onChange={(value) => setPayload((current) => ({ ...current, description: value }))}
            value={payload.description}
          />
          <TextField
            label="URL da capa"
            onChange={(value) => setPayload((current) => ({ ...current, coverImageUrl: value }))}
            value={payload.coverImageUrl ?? ''}
          />

          {message && <p className="rounded-md bg-red-50 p-3 text-sm text-red-700 dark:bg-red-950 dark:text-red-200">{message}</p>}

          <div className="flex flex-col gap-3 sm:flex-row sm:justify-between">
            {isEditing && canDelete ? (
              <button
                className="inline-flex items-center justify-center gap-2 rounded-md border border-red-300 px-4 py-3 text-sm font-semibold text-red-700 dark:border-red-900 dark:text-red-300"
                disabled={isSaving}
                onClick={deleteCampaign}
                type="button"
              >
                <Trash2 size={18} />
                Excluir
              </button>
            ) : (
              <span />
            )}
            <button
              className="inline-flex items-center justify-center gap-2 rounded-md bg-emerald-600 px-4 py-3 text-sm font-semibold text-white disabled:bg-slate-400"
              disabled={isSaving}
              type="submit"
            >
              {isSaving ? 'Salvando...' : 'Salvar campanha'}
            </button>
          </div>
        </form>
      )}
    </div>
  )
}

function CampaignDetailPage({ auth }: { auth: AuthContextValue }) {
  const { id } = useParams()
  const [campaign, setCampaign] = useState<CampaignResponse | null>(null)
  const [activeTab, setActiveTab] = useState<'overview' | 'master'>('overview')
  const [dashboard, setDashboard] = useState<CampaignMasterDashboardResponse | null>(null)
  const [message, setMessage] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const [isDashboardLoading, setIsDashboardLoading] = useState(false)

  useEffect(() => {
    if (!id) {
      return
    }

    apiRequest<CampaignResponse>(`/api/campaigns/${id}`, auth.token)
      .then(setCampaign)
      .catch((error) => setMessage(error instanceof Error ? error.message : 'Erro ao carregar campanha.'))
      .finally(() => setIsLoading(false))
  }, [auth.token, id])

  async function regenerateInvite() {
    if (!id) {
      return
    }

    setMessage('')
    try {
      setCampaign(await apiRequest<CampaignResponse>(`/api/campaigns/${id}/invite/regenerate`, auth.token, {
        method: 'POST',
      }))
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao regenerar convite.')
    }
  }

  async function copyInvite(code: string) {
    await navigator.clipboard?.writeText(code)
    setMessage('Código copiado.')
  }

  useEffect(() => {
    if (!id || !campaign || campaign.currentUserRole !== 'Master' || activeTab !== 'master') {
      return
    }

    let isCurrent = true

    async function loadDashboard() {
      setIsDashboardLoading(true)
      setMessage('')

      try {
        const data = await apiRequest<CampaignMasterDashboardResponse>(`/api/campaigns/${id}/master-dashboard`, auth.token)
        if (isCurrent) {
          setDashboard(data)
        }
      } catch (error) {
        if (isCurrent) {
          setMessage(error instanceof Error ? error.message : 'Erro ao carregar painel do mestre.')
        }
      } finally {
        if (isCurrent) {
          setIsDashboardLoading(false)
        }
      }
    }

    void loadDashboard()

    return () => {
      isCurrent = false
    }
  }, [activeTab, auth.token, campaign, id])

  if (isLoading) {
    return <PanelText>Carregando campanha...</PanelText>
  }

  if (!campaign) {
    return (
      <div className="mx-auto max-w-6xl space-y-4">
        <BackLink to="/campaigns" />
        <PanelText>{message || 'Campanha não encontrada.'}</PanelText>
      </div>
    )
  }

  const isMaster = campaign.currentUserRole === 'Master'

  return (
    <div className="mx-auto max-w-6xl space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
        <div>
          <BackLink to="/campaigns" />
          <p className="mt-5 text-sm font-semibold uppercase text-emerald-700 dark:text-emerald-400">
            {campaign.system}
          </p>
          <h2 className="mt-2 text-3xl font-semibold tracking-tight">{campaign.name}</h2>
          <div className="mt-3">
            <RoleBadge role={campaign.currentUserRole} />
          </div>
        </div>
        {isMaster && (
          <Link
            className="inline-flex items-center justify-center gap-2 rounded-md border border-slate-300 px-4 py-3 text-sm font-semibold text-slate-700 dark:border-slate-700 dark:text-slate-200"
            to={`/campaigns/${campaign.id}/edit`}
          >
            <Edit size={18} />
            Editar
          </Link>
        )}
      </div>

      {message && <p className="rounded-md bg-emerald-50 p-3 text-sm text-emerald-800 dark:bg-emerald-950 dark:text-emerald-200">{message}</p>}

      <div className="flex gap-2 overflow-x-auto border-b border-slate-200 dark:border-slate-800">
        <TabButton active={activeTab === 'overview'} onClick={() => setActiveTab('overview')}>
          Visão Geral
        </TabButton>
        {isMaster && (
          <TabButton active={activeTab === 'master'} onClick={() => setActiveTab('master')}>
            Painel do Mestre
          </TabButton>
        )}
      </div>

      {activeTab === 'master' && isMaster ? (
        <CampaignMasterDashboard dashboard={dashboard} isLoading={isDashboardLoading} />
      ) : (
        <>
          <section className="grid gap-4 lg:grid-cols-[1fr_320px]">
            <div className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
              <h3 className="text-lg font-semibold">Resumo</h3>
              <p className="mt-3 whitespace-pre-wrap text-sm leading-6 text-slate-600 dark:text-slate-300">
                {campaign.description || 'Sem descrição.'}
              </p>
            </div>

            {isMaster && (
          <div className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
            <h3 className="text-lg font-semibold">Convite</h3>
            <p className="mt-2 text-sm text-slate-600 dark:text-slate-300">
              Compartilhe este código para jogadores entrarem.
            </p>
            <div className="mt-4 flex items-center gap-2">
              <code className="flex-1 rounded-md bg-slate-100 px-3 py-2 text-center font-mono text-lg font-semibold tracking-widest dark:bg-slate-950">
                {campaign.inviteCode}
              </code>
              <button
                aria-label="Copiar convite"
                className="flex size-10 items-center justify-center rounded-md border border-slate-300 dark:border-slate-700"
                onClick={() => copyInvite(campaign.inviteCode)}
                type="button"
              >
                <Copy size={17} />
              </button>
            </div>
            <button
              className="mt-3 inline-flex w-full items-center justify-center gap-2 rounded-md border border-slate-300 px-4 py-2.5 text-sm font-semibold text-slate-700 dark:border-slate-700 dark:text-slate-200"
              onClick={regenerateInvite}
              type="button"
            >
              <RefreshCw size={17} />
              Regenerar convite
            </button>
          </div>
            )}
          </section>

          <section className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
            <h3 className="text-lg font-semibold">{isMaster ? 'Membros' : 'Membros públicos'}</h3>
            <div className="mt-4 divide-y divide-slate-200 dark:divide-slate-800">
              {campaign.members.map((member) => (
                <div className="flex items-center justify-between gap-3 py-3" key={member.id}>
                  <div className="min-w-0">
                    <p className="truncate font-medium">{member.userName}</p>
                    {isMaster && (
                      <p className="truncate text-sm text-slate-500 dark:text-slate-400">{member.email}</p>
                    )}
                  </div>
                  <RoleBadge role={member.role} />
                </div>
              ))}
            </div>
          </section>
        </>
      )}
    </div>
  )
}

function CampaignMasterDashboard({
  dashboard,
  isLoading,
}: {
  dashboard: CampaignMasterDashboardResponse | null
  isLoading: boolean
}) {
  if (isLoading) {
    return <PanelText>Carregando painel do mestre...</PanelText>
  }

  if (!dashboard) {
    return <PanelText>Painel não carregado.</PanelText>
  }

  return (
    <section className="space-y-6">
      <div className="grid gap-3 sm:grid-cols-3">
        <StatPill label="Personagens" value={dashboard.characters.length.toString()} />
        <StatPill label="Membros" value={dashboard.members.length.toString()} />
        <StatPill label="Notas visíveis" value={dashboard.visibleNotes.length.toString()} />
      </div>

      <div>
        <h3 className="text-xl font-semibold">Personagens da campanha</h3>
        {dashboard.characters.length === 0 ? (
          <PanelText>Nenhum personagem vinculado à campanha.</PanelText>
        ) : (
          <div className="mt-4 grid gap-4 lg:grid-cols-2">
            {dashboard.characters.map((character) => (
              <div className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900" key={character.id}>
                <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
                  <div>
                    <h4 className="text-lg font-semibold">{character.name}</h4>
                    <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">{character.userName}</p>
                  </div>
                  <Link
                    className="inline-flex items-center justify-center gap-2 rounded-md border border-slate-300 px-3 py-2 text-sm font-semibold text-slate-700 dark:border-slate-700 dark:text-slate-200"
                    to={`/characters/${character.id}`}
                  >
                    Abrir ficha
                  </Link>
                </div>
                <div className="mt-4 grid gap-3 sm:grid-cols-2">
                  <InfoRow label="Classe" value={character.mainClass || '-'} />
                  <InfoRow label="Nível" value={character.totalLevel.toString()} />
                  <InfoRow label="PV" value={`${character.currentHitPoints}/${character.maxHitPoints}`} />
                  <InfoRow label="CA" value={character.armorClass.toString()} />
                  <InfoRow label="Percepção passiva" value={character.passivePerception.toString()} />
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      <div className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
        <h3 className="text-xl font-semibold">Notas visíveis para o mestre</h3>
        {dashboard.visibleNotes.length === 0 ? (
          <p className="mt-3 text-sm text-slate-500 dark:text-slate-400">Nenhuma nota visível.</p>
        ) : (
          <div className="mt-4 grid gap-3">
            {dashboard.visibleNotes.map((note) => (
              <article className="rounded-md border border-slate-200 p-4 dark:border-slate-800" key={note.id}>
                <div className="flex flex-col gap-1 sm:flex-row sm:items-start sm:justify-between">
                  <div>
                    <h4 className="font-semibold">{note.title}</h4>
                    <p className="text-sm text-slate-500 dark:text-slate-400">{note.characterName} · {note.category || 'Outros'}</p>
                  </div>
                </div>
                <p className="mt-3 whitespace-pre-wrap text-sm leading-6 text-slate-600 dark:text-slate-300">{note.content}</p>
                {note.tags && <p className="mt-2 text-xs text-slate-500 dark:text-slate-400">{note.tags}</p>}
              </article>
            ))}
          </div>
        )}
      </div>
    </section>
  )
}

const emptyCharacter: CharacterPayload = {
  campaignId: null,
  name: '',
  nickname: '',
  avatarUrl: '',
  tokenImageUrl: '',
  totalLevel: 1,
  species: '',
  mainClass: '',
  subclass: '',
  background: '',
  alignment: '',
  experience: 0,
  inspiration: false,
  proficiencyBonus: 2,
  armorClass: 10,
  initiative: 0,
  speed: 9,
  maxHitPoints: 0,
  currentHitPoints: 0,
  temporaryHitPoints: 0,
  totalHitDice: '',
  availableHitDice: '',
  physicalDescription: '',
  personalityTraits: '',
  ideals: '',
  bonds: '',
  flaws: '',
  backstory: '',
  quickNotes: '',
}

function CharactersPage({ auth }: { auth: AuthContextValue }) {
  const [characters, setCharacters] = useState<CharacterSummaryResponse[]>([])
  const [message, setMessage] = useState('')
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    apiRequest<CharacterSummaryResponse[]>('/api/characters', auth.token)
      .then(setCharacters)
      .catch((error) => setMessage(error instanceof Error ? error.message : 'Erro ao carregar personagens.'))
      .finally(() => setIsLoading(false))
  }, [auth.token])

  return (
    <div className="mx-auto max-w-6xl space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <p className="text-sm font-semibold uppercase text-emerald-700 dark:text-emerald-400">Personagens</p>
          <h2 className="mt-2 text-3xl font-semibold tracking-tight">Meus Personagens</h2>
          <p className="mt-2 max-w-2xl text-slate-600 dark:text-slate-300">
            Crie fichas básicas, vincule campanhas e acompanhe dados gerais.
          </p>
        </div>
        <Link
          className="inline-flex items-center justify-center gap-2 rounded-md bg-emerald-600 px-4 py-3 text-sm font-semibold text-white"
          to="/characters/new"
        >
          <Plus size={18} />
          Novo personagem
        </Link>
      </div>

      {message && <p className="rounded-md bg-red-50 p-3 text-sm text-red-700 dark:bg-red-950 dark:text-red-200">{message}</p>}

      {isLoading ? (
        <PanelText>Carregando personagens...</PanelText>
      ) : characters.length === 0 ? (
        <PanelText>Nenhum personagem ainda. Crie sua primeira ficha básica.</PanelText>
      ) : (
        <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          {characters.map((character) => (
            <Link
              className="overflow-hidden rounded-lg border border-slate-200 bg-white shadow-sm transition hover:-translate-y-0.5 hover:border-emerald-300 hover:shadow-md dark:border-slate-800 dark:bg-slate-900 dark:hover:border-emerald-700"
              key={character.id}
              to={`/characters/${character.id}`}
            >
              <div className="flex gap-4 p-5">
                <AvatarImage name={character.name} src={character.avatarUrl} />
                <div className="min-w-0 flex-1">
                  <div className="flex items-start justify-between gap-2">
                    <div className="min-w-0">
                      <h3 className="truncate text-lg font-semibold">{character.name}</h3>
                      <p className="truncate text-sm text-slate-500 dark:text-slate-400">
                        {character.nickname || character.species || 'Sem apelido'}
                      </p>
                    </div>
                    {!character.canEdit && <RoleBadge role="Master" />}
                  </div>
                  <p className="mt-3 text-sm text-slate-700 dark:text-slate-200">
                    Nível {character.totalLevel} · {character.mainClass || 'Classe não definida'}
                  </p>
                  <p className="mt-1 truncate text-sm text-slate-500 dark:text-slate-400">
                    {character.campaignName ?? 'Sem campanha'}
                  </p>
                  <div className="mt-4 grid grid-cols-2 gap-2 text-sm">
                    <StatPill label="PV" value={`${character.currentHitPoints}/${character.maxHitPoints}`} />
                    <StatPill label="CA" value={character.armorClass.toString()} />
                  </div>
                </div>
              </div>
            </Link>
          ))}
        </section>
      )}
    </div>
  )
}

function CharacterFormPage({ auth }: { auth: AuthContextValue }) {
  const { id } = useParams()
  const isEditing = Boolean(id)
  const navigate = useNavigate()
  const [payload, setPayload] = useState<CharacterPayload>(emptyCharacter)
  const [campaigns, setCampaigns] = useState<CampaignSummaryResponse[]>([])
  const [message, setMessage] = useState('')
  const [isLoading, setIsLoading] = useState(isEditing)
  const [isSaving, setIsSaving] = useState(false)
  const [canDelete, setCanDelete] = useState(false)
  const [canEdit, setCanEdit] = useState(!isEditing)

  useEffect(() => {
    apiRequest<CampaignSummaryResponse[]>('/api/campaigns', auth.token)
      .then(setCampaigns)
      .catch(() => setCampaigns([]))
  }, [auth.token])

  useEffect(() => {
    if (!id) {
      return
    }

    apiRequest<CharacterResponse>(`/api/characters/${id}`, auth.token)
      .then((character) => {
        setPayload(toCharacterPayload(character))
        setCanDelete(character.canEdit)
        setCanEdit(character.canEdit)
      })
      .catch((error) => setMessage(error instanceof Error ? error.message : 'Erro ao carregar personagem.'))
      .finally(() => setIsLoading(false))
  }, [auth.token, id])

  function patchPayload(patch: Partial<CharacterPayload>) {
    setPayload((current) => ({ ...current, ...patch }))
  }

  async function saveCharacter(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setIsSaving(true)
    setMessage('')

    try {
      const character = await apiRequest<CharacterResponse>(
        isEditing ? `/api/characters/${id}` : '/api/characters',
        auth.token,
        {
          method: isEditing ? 'PUT' : 'POST',
          body: JSON.stringify(normalizeCharacterPayload(payload)),
        },
      )
      navigate(`/characters/${character.id}`)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao salvar personagem.')
    } finally {
      setIsSaving(false)
    }
  }

  async function deleteCharacter() {
    if (!id) {
      return
    }

    setIsSaving(true)
    setMessage('')
    try {
      await apiRequest<null>(`/api/characters/${id}`, auth.token, { method: 'DELETE' })
      navigate('/characters')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao excluir personagem.')
    } finally {
      setIsSaving(false)
    }
  }

  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <BackLink to={id ? `/characters/${id}` : '/characters'} />
      <div>
        <p className="text-sm font-semibold uppercase text-emerald-700 dark:text-emerald-400">Personagens</p>
        <h2 className="mt-2 text-3xl font-semibold tracking-tight">
          {isEditing ? 'Editar personagem' : 'Criar personagem'}
        </h2>
      </div>

      {isLoading ? (
        <PanelText>Carregando personagem...</PanelText>
      ) : !canEdit ? (
        <AccessDenied backTo={`/characters/${id}`} />
      ) : (
        <form
          className="space-y-5 rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900"
          onSubmit={saveCharacter}
        >
          <section className="grid gap-4 md:grid-cols-2">
            <TextField label="Nome" onChange={(value) => patchPayload({ name: value })} required value={payload.name} />
            <TextField label="Apelido" onChange={(value) => patchPayload({ nickname: value })} value={payload.nickname ?? ''} />
            <TextField label="Espécie/Raça" onChange={(value) => patchPayload({ species: value })} value={payload.species} />
            <TextField label="Classe principal" onChange={(value) => patchPayload({ mainClass: value })} value={payload.mainClass} />
            <TextField label="Subclasse" onChange={(value) => patchPayload({ subclass: value })} value={payload.subclass} />
            <TextField label="Antecedente" onChange={(value) => patchPayload({ background: value })} value={payload.background} />
            <TextField label="Alinhamento" onChange={(value) => patchPayload({ alignment: value })} value={payload.alignment} />
            <label className="block">
              <span className="text-sm font-medium text-slate-700 dark:text-slate-200">Campanha</span>
              <select
                className="mt-1 min-h-11 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
                onChange={(event) => patchPayload({ campaignId: event.target.value || null })}
                value={payload.campaignId ?? ''}
              >
                <option value="">Sem campanha</option>
                {campaigns.map((campaign) => (
                  <option key={campaign.id} value={campaign.id}>
                    {campaign.name}
                  </option>
                ))}
              </select>
            </label>
          </section>

          <section className="grid gap-4 md:grid-cols-3">
            <NumberField label="Nível total" min={1} onChange={(value) => patchPayload({ totalLevel: value })} value={payload.totalLevel} />
            <NumberField label="Experiência" min={0} onChange={(value) => patchPayload({ experience: value })} value={payload.experience} />
            <NumberField label="Bônus proficiência" min={0} onChange={(value) => patchPayload({ proficiencyBonus: value })} value={payload.proficiencyBonus} />
            <NumberField label="Classe de Armadura" min={0} onChange={(value) => patchPayload({ armorClass: value })} value={payload.armorClass} />
            <NumberField label="Iniciativa" onChange={(value) => patchPayload({ initiative: value })} value={payload.initiative} />
            <NumberField label="Deslocamento" min={0} onChange={(value) => patchPayload({ speed: value })} value={payload.speed} />
            <NumberField label="Vida máxima" min={0} onChange={(value) => patchPayload({ maxHitPoints: value })} value={payload.maxHitPoints} />
            <NumberField label="Vida atual" min={0} onChange={(value) => patchPayload({ currentHitPoints: value })} value={payload.currentHitPoints} />
            <NumberField label="Vida temporária" min={0} onChange={(value) => patchPayload({ temporaryHitPoints: value })} value={payload.temporaryHitPoints} />
          </section>

          <section className="grid gap-4 md:grid-cols-2">
            <TextField label="Dados de vida totais" onChange={(value) => patchPayload({ totalHitDice: value })} value={payload.totalHitDice} />
            <TextField label="Dados de vida disponíveis" onChange={(value) => patchPayload({ availableHitDice: value })} value={payload.availableHitDice} />
            <TextField label="URL do avatar" onChange={(value) => patchPayload({ avatarUrl: value })} value={payload.avatarUrl ?? ''} />
            <TextField label="URL do token" onChange={(value) => patchPayload({ tokenImageUrl: value })} value={payload.tokenImageUrl ?? ''} />
          </section>

          <label className="flex items-center gap-2 text-sm font-medium text-slate-700 dark:text-slate-200">
            <input
              checked={payload.inspiration}
              className="size-4"
              onChange={(event) => patchPayload({ inspiration: event.target.checked })}
              type="checkbox"
            />
            Inspiração
          </label>

          <section className="grid gap-4 md:grid-cols-2">
            <TextAreaField label="Descrição física" onChange={(value) => patchPayload({ physicalDescription: value })} value={payload.physicalDescription} />
            <TextAreaField label="Personalidade" onChange={(value) => patchPayload({ personalityTraits: value })} value={payload.personalityTraits} />
            <TextAreaField label="Ideais" onChange={(value) => patchPayload({ ideals: value })} value={payload.ideals} />
            <TextAreaField label="Vínculos" onChange={(value) => patchPayload({ bonds: value })} value={payload.bonds} />
            <TextAreaField label="Defeitos" onChange={(value) => patchPayload({ flaws: value })} value={payload.flaws} />
            <TextAreaField label="Anotações rápidas" onChange={(value) => patchPayload({ quickNotes: value })} value={payload.quickNotes} />
          </section>

          <TextAreaField label="História" onChange={(value) => patchPayload({ backstory: value })} value={payload.backstory} />

          {message && <p className="rounded-md bg-red-50 p-3 text-sm text-red-700 dark:bg-red-950 dark:text-red-200">{message}</p>}

          <div className="flex flex-col gap-3 sm:flex-row sm:justify-between">
            {isEditing && canDelete ? (
              <button
                className="inline-flex items-center justify-center gap-2 rounded-md border border-red-300 px-4 py-3 text-sm font-semibold text-red-700 dark:border-red-900 dark:text-red-300"
                disabled={isSaving}
                onClick={deleteCharacter}
                type="button"
              >
                <Trash2 size={18} />
                Excluir
              </button>
            ) : (
              <span />
            )}
            <button
              className="inline-flex items-center justify-center gap-2 rounded-md bg-emerald-600 px-4 py-3 text-sm font-semibold text-white disabled:bg-slate-400"
              disabled={isSaving}
              type="submit"
            >
              {isSaving ? 'Salvando...' : 'Salvar personagem'}
            </button>
          </div>
        </form>
      )}
    </div>
  )
}

function CharacterDetailPage({ auth }: { auth: AuthContextValue }) {
  const { id } = useParams()
  const [character, setCharacter] = useState<CharacterResponse | null>(null)
  const [activeTab, setActiveTab] = useState<'overview' | 'attributes' | 'skills' | 'combat' | 'spells' | 'features' | 'notebook' | 'inventory' | 'assets'>('overview')
  const [diceHistory, setDiceHistory] = useState<DiceRollResponse[]>([])
  const [diceMessage, setDiceMessage] = useState('')
  const [restDialog, setRestDialog] = useState<RestType | null>(null)
  const [restOptions, setRestOptions] = useState<CharacterRestRequest>({ restoreHitPoints: true, restoreHitDice: true })
  const [restVersion, setRestVersion] = useState(0)
  const [message, setMessage] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const [isResting, setIsResting] = useState(false)
  const [isExportingPdf, setIsExportingPdf] = useState(false)

  useEffect(() => {
    if (!id) {
      return
    }

    apiRequest<CharacterResponse>(`/api/characters/${id}`, auth.token)
      .then(setCharacter)
      .catch((error) => setMessage(error instanceof Error ? error.message : 'Erro ao carregar personagem.'))
      .finally(() => setIsLoading(false))
  }, [auth.token, id])

  if (isLoading) {
    return <PanelText>Carregando personagem...</PanelText>
  }

  if (!character) {
    return (
      <div className="mx-auto max-w-6xl space-y-4">
        <BackLink to="/characters" />
        <PanelText>{message || 'Personagem não encontrado.'}</PanelText>
      </div>
    )
  }

  async function rollDice(request: DiceRollRequest) {
    setDiceMessage('')
    try {
      const result = await apiRequest<DiceRollResponse>('/api/dice/roll', auth.token, {
        method: 'POST',
        body: JSON.stringify(request),
      })
      setDiceHistory((current) => [result, ...current].slice(0, 12))
      return result
    } catch (error) {
      setDiceMessage(error instanceof Error ? error.message : 'Erro ao rolar dados.')
      return null
    }
  }

  function openRestDialog(type: RestType) {
    setRestOptions({
      restoreHitPoints: type === 'long',
      restoreHitDice: type === 'long',
    })
    setRestDialog(type)
    setMessage('')
  }

  async function performRest() {
    if (!restDialog || !character) {
      return
    }

    setIsResting(true)
    setMessage('')
    try {
      const result = await apiRequest<CharacterRestResponse>(
        `/api/characters/${character.id}/${restDialog === 'short' ? 'short-rest' : 'long-rest'}`,
        auth.token,
        {
          method: 'POST',
          body: JSON.stringify(restOptions),
        },
      )
      setCharacter(result.character)
      setRestVersion((current) => current + 1)
      setRestDialog(null)
      setMessage(restDialog === 'short' ? 'Descanso curto aplicado.' : 'Descanso longo aplicado.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao aplicar descanso.')
    } finally {
      setIsResting(false)
    }
  }

  async function exportPdf() {
    if (!character) {
      return
    }

    const pdfWindow = window.open('', '_blank')
    if (!pdfWindow) {
      setMessage('O navegador bloqueou a janela do PDF. Permita pop-ups para esta página e tente novamente.')
      return
    }
    pdfWindow.document.write('<p style="font-family: system-ui, sans-serif; padding: 24px;">Gerando PDF...</p>')

    setIsExportingPdf(true)
    setMessage('')

    try {
      const [abilities, skills, combat, attacks, spells, features, inventory, campaignCharacters] = await Promise.all([
        safeApiRequest<AbilityScoreResponse[]>(`/api/characters/${character.id}/attributes`, auth.token, []),
        safeApiRequest<CharacterSkillResponse[]>(`/api/characters/${character.id}/skills`, auth.token, []),
        safeApiRequest<CharacterCombatResponse | null>(`/api/characters/${character.id}/combat`, auth.token, null),
        safeApiRequest<CharacterAttackResponse[]>(`/api/characters/${character.id}/attacks`, auth.token, []),
        safeApiRequest<CharacterSpellResponse[]>(`/api/characters/${character.id}/spells`, auth.token, []),
        safeApiRequest<CharacterFeatureResponse[]>(`/api/characters/${character.id}/features`, auth.token, []),
        safeApiRequest<CharacterInventoryItemResponse[]>(`/api/characters/${character.id}/inventory`, auth.token, []),
        character.campaignId
          ? safeApiRequest<CampaignCharacterSummaryResponse[]>(`/api/campaigns/${character.campaignId}/characters`, auth.token, [])
          : Promise.resolve([]),
      ])

      const playerName =
        campaignCharacters.find((item) => item.id === character.id)?.userName
        ?? (character.userId === auth.user?.id ? auth.user.name : 'Não informado')

      openCharacterPdf(pdfWindow, {
        character,
        playerName,
        abilities,
        skills,
        combat,
        attacks,
        spells,
        features,
        inventory,
      })
    } catch (error) {
      pdfWindow.close()
      setMessage(error instanceof Error ? error.message : 'Erro ao gerar PDF.')
    } finally {
      setIsExportingPdf(false)
    }
  }

  return (
    <div className="mx-auto max-w-6xl space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
        <div className="flex gap-4">
          <AvatarImage name={character.name} src={character.avatarUrl} />
          <div>
            <BackLink to="/characters" />
            <h2 className="mt-4 text-3xl font-semibold tracking-tight">{character.name}</h2>
            <p className="mt-1 text-slate-600 dark:text-slate-300">
              {character.nickname || character.species || 'Ficha básica'}
            </p>
            <p className="mt-2 text-sm text-slate-500 dark:text-slate-400">
              {character.campaignName ?? 'Sem campanha'}
            </p>
          </div>
        </div>
        <div className="flex flex-col gap-2 sm:flex-row">
          <button
            className="inline-flex items-center justify-center gap-2 rounded-md border border-slate-300 px-4 py-3 text-sm font-semibold text-slate-700 dark:border-slate-700 dark:text-slate-200"
            disabled={isExportingPdf}
            onClick={exportPdf}
            type="button"
          >
            <Download size={18} />
            {isExportingPdf ? 'Gerando...' : 'Exportar PDF'}
          </button>
          {character.canEdit && (
            <>
            <button
              className="inline-flex items-center justify-center gap-2 rounded-md border border-slate-300 px-4 py-3 text-sm font-semibold text-slate-700 dark:border-slate-700 dark:text-slate-200"
              onClick={() => openRestDialog('short')}
              type="button"
            >
              Descanso curto
            </button>
            <button
              className="inline-flex items-center justify-center gap-2 rounded-md border border-slate-300 px-4 py-3 text-sm font-semibold text-slate-700 dark:border-slate-700 dark:text-slate-200"
              onClick={() => openRestDialog('long')}
              type="button"
            >
              Descanso longo
            </button>
            <Link
              className="inline-flex items-center justify-center gap-2 rounded-md border border-slate-300 px-4 py-3 text-sm font-semibold text-slate-700 dark:border-slate-700 dark:text-slate-200"
              to={`/characters/${character.id}/edit`}
            >
              <Edit size={18} />
              Editar
            </Link>
            </>
          )}
        </div>
      </div>

      {message && (
        <p className="rounded-md bg-emerald-50 p-3 text-sm text-emerald-800 dark:bg-emerald-950 dark:text-emerald-200">
          {message}
        </p>
      )}

      <section className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard icon={Swords} label="Classe" value={`${character.mainClass || '-'} ${character.totalLevel}`} />
        <StatCard icon={ShieldCheck} label="CA" value={character.armorClass.toString()} />
        <StatCard icon={Heart} label="PV" value={`${character.currentHitPoints}/${character.maxHitPoints}`} />
        <StatCard icon={IdCard} label="Bônus prof." value={`+${character.proficiencyBonus}`} />
      </section>

      <DiceRoller history={diceHistory} message={diceMessage} onRoll={rollDice} />

      <div className="flex gap-2 overflow-x-auto border-b border-slate-200 dark:border-slate-800">
        <TabButton active={activeTab === 'overview'} onClick={() => setActiveTab('overview')}>
          Visão Geral
        </TabButton>
        <TabButton active={activeTab === 'attributes'} onClick={() => setActiveTab('attributes')}>
          Atributos
        </TabButton>
        <TabButton active={activeTab === 'skills'} onClick={() => setActiveTab('skills')}>
          Perícias
        </TabButton>
        <TabButton active={activeTab === 'combat'} onClick={() => setActiveTab('combat')}>
          Combate
        </TabButton>
        <TabButton active={activeTab === 'spells'} onClick={() => setActiveTab('spells')}>
          Magias
        </TabButton>
        <TabButton active={activeTab === 'features'} onClick={() => setActiveTab('features')}>
          Talentos
        </TabButton>
        <TabButton active={activeTab === 'notebook'} onClick={() => setActiveTab('notebook')}>
          Notebook
        </TabButton>
        <TabButton active={activeTab === 'inventory'} onClick={() => setActiveTab('inventory')}>
          Inventário
        </TabButton>
        <TabButton active={activeTab === 'assets'} onClick={() => setActiveTab('assets')}>
          Imagens
        </TabButton>
      </div>

      {activeTab === 'overview' ? (
        <>
          <section className="grid gap-4 lg:grid-cols-2">
            <InfoPanel title="Dados gerais">
              <InfoRow label="Espécie/Raça" value={character.species} />
              <InfoRow label="Subclasse" value={character.subclass} />
              <InfoRow label="Antecedente" value={character.background} />
              <InfoRow label="Alinhamento" value={character.alignment} />
              <InfoRow label="Experiência" value={character.experience.toString()} />
              <InfoRow label="Inspiração" value={character.inspiration ? 'Sim' : 'Não'} />
              <InfoRow label="Iniciativa" value={character.initiative.toString()} />
              <InfoRow label="Deslocamento" value={character.speed.toString()} />
              <InfoRow label="Vida temporária" value={character.temporaryHitPoints.toString()} />
              <InfoRow label="Dados de vida" value={`${character.availableHitDice || '-'} / ${character.totalHitDice || '-'}`} />
            </InfoPanel>
            <InfoPanel title="Descrição">
              <TextBlock label="Descrição física" value={character.physicalDescription} />
              <TextBlock label="Personalidade" value={character.personalityTraits} />
              <TextBlock label="Ideais" value={character.ideals} />
              <TextBlock label="Vínculos" value={character.bonds} />
              <TextBlock label="Defeitos" value={character.flaws} />
            </InfoPanel>
          </section>

          <section className="grid gap-4 lg:grid-cols-2">
            <InfoPanel title="História">
              <p className="whitespace-pre-wrap text-sm leading-6 text-slate-600 dark:text-slate-300">
                {character.backstory || 'Sem história.'}
              </p>
            </InfoPanel>
            <InfoPanel title="Anotações rápidas">
              <p className="whitespace-pre-wrap text-sm leading-6 text-slate-600 dark:text-slate-300">
                {character.quickNotes || 'Sem anotações.'}
              </p>
            </InfoPanel>
          </section>
        </>
      ) : activeTab === 'attributes' ? (
        <CharacterAttributesSection auth={auth} character={character} onRoll={rollDice} />
      ) : activeTab === 'skills' ? (
        <CharacterSkillsSection auth={auth} character={character} onRoll={rollDice} />
      ) : activeTab === 'combat' ? (
        <CharacterCombatSection auth={auth} character={character} onRoll={rollDice} restVersion={restVersion} />
      ) : activeTab === 'spells' ? (
        <CharacterSpellsSection auth={auth} character={character} restVersion={restVersion} />
      ) : activeTab === 'features' ? (
        <CharacterFeaturesSection auth={auth} character={character} restVersion={restVersion} />
      ) : activeTab === 'notebook' ? (
        <CharacterNotebookSection auth={auth} character={character} />
      ) : activeTab === 'inventory' ? (
        <CharacterInventorySection auth={auth} character={character} />
      ) : (
        <CharacterAssetsSection auth={auth} character={character} onCharacterChange={setCharacter} />
      )}

      {restDialog && (
        <RestConfirmModal
          isSaving={isResting}
          onCancel={() => setRestDialog(null)}
          onConfirm={performRest}
          onOptionsChange={setRestOptions}
          options={restOptions}
          type={restDialog}
        />
      )}
    </div>
  )
}

function RestConfirmModal({
  isSaving,
  onCancel,
  onConfirm,
  onOptionsChange,
  options,
  type,
}: {
  isSaving: boolean
  onCancel: () => void
  onConfirm: () => void
  onOptionsChange: (options: CharacterRestRequest) => void
  options: CharacterRestRequest
  type: RestType
}) {
  const isLongRest = type === 'long'

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/60 p-4">
      <div className="w-full max-w-lg rounded-lg border border-slate-200 bg-white p-5 shadow-xl dark:border-slate-800 dark:bg-slate-900">
        <h3 className="text-xl font-semibold">{isLongRest ? 'Confirmar descanso longo' : 'Confirmar descanso curto'}</h3>
        <div className="mt-4 space-y-3 text-sm text-slate-600 dark:text-slate-300">
          {isLongRest ? (
            <>
              <p>Recupera recursos com recuperação em descanso curto ou longo.</p>
              <p>Restaura slots de magia, zerando usos gastos.</p>
            </>
          ) : (
            <>
              <p>Recupera recursos com recuperação em descanso curto.</p>
              <p>Não altera vida nem dados de vida automaticamente.</p>
            </>
          )}
        </div>

        {isLongRest && (
          <div className="mt-4 grid gap-3">
            <label className="flex items-center gap-2 text-sm">
              <input
                checked={options.restoreHitPoints}
                className="size-4"
                onChange={(event) => onOptionsChange({ ...options, restoreHitPoints: event.target.checked })}
                type="checkbox"
              />
              Restaurar PV para o máximo e zerar PV temporário
            </label>
            <label className="flex items-center gap-2 text-sm">
              <input
                checked={options.restoreHitDice}
                className="size-4"
                onChange={(event) => onOptionsChange({ ...options, restoreHitDice: event.target.checked })}
                type="checkbox"
              />
              Restaurar dados de vida disponíveis para o total
            </label>
          </div>
        )}

        <div className="mt-6 flex flex-col gap-3 sm:flex-row sm:justify-end">
          <button
            className="rounded-md border border-slate-300 px-4 py-3 text-sm font-semibold text-slate-700 dark:border-slate-700 dark:text-slate-200"
            disabled={isSaving}
            onClick={onCancel}
            type="button"
          >
            Cancelar
          </button>
          <button
            className="rounded-md bg-emerald-600 px-4 py-3 text-sm font-semibold text-white disabled:bg-slate-400"
            disabled={isSaving}
            onClick={onConfirm}
            type="button"
          >
            {isSaving ? 'Aplicando...' : 'Aplicar descanso'}
          </button>
        </div>
      </div>
    </div>
  )
}

function DiceRoller({
  history,
  message,
  onRoll,
}: {
  history: DiceRollResponse[]
  message: string
  onRoll: RollDiceFn
}) {
  const [expression, setExpression] = useState('1d20')
  const [label, setLabel] = useState('')
  const [advantage, setAdvantage] = useState(false)
  const [disadvantage, setDisadvantage] = useState(false)
  const [isRolling, setIsRolling] = useState(false)

  async function submitRoll(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    await roll({ expression, label: label || null, advantage, disadvantage })
  }

  async function roll(request: DiceRollRequest) {
    setIsRolling(true)
    try {
      await onRoll(request)
    } finally {
      setIsRolling(false)
    }
  }

  const latest = history[0]

  return (
    <section className="rounded-lg border border-slate-200 bg-white p-4 dark:border-slate-800 dark:bg-slate-900">
      <div className="flex flex-col gap-4 xl:flex-row xl:items-start xl:justify-between">
        <div className="min-w-0 flex-1">
          <div className="flex items-center gap-2">
            <Dices size={20} />
            <h3 className="text-lg font-semibold">Rolador de dados</h3>
          </div>
          <form className="mt-3 grid gap-3 lg:grid-cols-[140px_1fr_auto_auto_auto] lg:items-end" onSubmit={submitRoll}>
            <TextField label="Expressão" onChange={setExpression} value={expression} />
            <TextField label="Rótulo" onChange={setLabel} value={label} />
            <label className="flex items-center gap-2 rounded-md border border-slate-200 px-3 py-2 text-sm dark:border-slate-800">
              <input
                checked={advantage}
                className="size-4"
                onChange={(event) => {
                  setAdvantage(event.target.checked)
                  if (event.target.checked) {
                    setDisadvantage(false)
                  }
                }}
                type="checkbox"
              />
              Vantagem
            </label>
            <label className="flex items-center gap-2 rounded-md border border-slate-200 px-3 py-2 text-sm dark:border-slate-800">
              <input
                checked={disadvantage}
                className="size-4"
                onChange={(event) => {
                  setDisadvantage(event.target.checked)
                  if (event.target.checked) {
                    setAdvantage(false)
                  }
                }}
                type="checkbox"
              />
              Desvantagem
            </label>
            <button className="inline-flex items-center justify-center gap-2 rounded-md bg-emerald-600 px-4 py-2.5 text-sm font-semibold text-white disabled:bg-slate-400" disabled={isRolling} type="submit">
              <Dices size={17} />
              Rolar
            </button>
          </form>
          <div className="mt-3 flex flex-wrap gap-2">
            {[4, 6, 8, 10, 12, 20, 100].map((sides) => (
              <button
                className="rounded-md border border-slate-300 px-3 py-2 text-sm font-semibold dark:border-slate-700"
                disabled={isRolling}
                key={sides}
                onClick={() => roll({ expression: `1d${sides}`, advantage: false, disadvantage: false, label: `d${sides}` })}
                type="button"
              >
                d{sides}
              </button>
            ))}
          </div>
          {message && <p className="mt-3 rounded-md bg-red-50 p-3 text-sm text-red-700 dark:bg-red-950 dark:text-red-200">{message}</p>}
        </div>

        <div className="min-w-0 xl:w-96">
          {latest ? (
            <div className="rounded-md bg-slate-100 p-4 dark:bg-slate-950">
              <p className="text-sm text-slate-500 dark:text-slate-400">{latest.label || latest.expression}</p>
              <p className="mt-1 text-4xl font-semibold">{latest.total}</p>
              <p className="mt-2 text-sm text-slate-600 dark:text-slate-300">
                {latest.expression}: [{latest.rolls.join(', ')}]{latest.modifier ? ` ${formatSigned(latest.modifier)}` : ''}
              </p>
            </div>
          ) : (
            <p className="rounded-md bg-slate-100 p-4 text-sm text-slate-500 dark:bg-slate-950 dark:text-slate-400">Sem rolagens ainda.</p>
          )}
          {history.length > 1 && (
            <div className="mt-3 max-h-44 space-y-2 overflow-y-auto">
              {history.slice(1).map((roll, index) => (
                <div className="flex items-center justify-between gap-3 rounded-md border border-slate-200 px-3 py-2 text-sm dark:border-slate-800" key={`${roll.rolledAt}-${index}`}>
                  <span className="truncate">{roll.label || roll.expression}</span>
                  <span className="font-semibold">{roll.total}</span>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </section>
  )
}

function CharacterAttributesSection({
  auth,
  character,
  onRoll,
}: {
  auth: AuthContextValue
  character: CharacterResponse
  onRoll: RollDiceFn
}) {
  const [abilities, setAbilities] = useState<AbilityScoreResponse[]>([])
  const [savingThrows, setSavingThrows] = useState<SavingThrowResponse[]>([])
  const [message, setMessage] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)

  useEffect(() => {
    Promise.all([
      apiRequest<AbilityScoreResponse[]>(`/api/characters/${character.id}/attributes`, auth.token),
      apiRequest<SavingThrowResponse[]>(`/api/characters/${character.id}/saving-throws`, auth.token),
    ])
      .then(([abilityData, savingThrowData]) => {
        setAbilities(abilityData)
        setSavingThrows(savingThrowData)
      })
      .catch((error) => setMessage(error instanceof Error ? error.message : 'Erro ao carregar atributos.'))
      .finally(() => setIsLoading(false))
  }, [auth.token, character.id])

  function updateAbility(attribute: AbilityType, score: number) {
    setAbilities((current) =>
      current.map((ability) =>
        ability.attribute === attribute
          ? { ...ability, score, modifier: calculateModifier(score) }
          : ability,
      ),
    )
    setSavingThrows((current) =>
      current.map((savingThrow) =>
        savingThrow.attribute === attribute
          ? {
              ...savingThrow,
              modifier: calculateModifier(score),
              finalValue:
                calculateModifier(score)
                + savingThrow.customBonus
                + (savingThrow.isProficient ? character.proficiencyBonus : 0),
            }
          : savingThrow,
      ),
    )
  }

  function updateSavingThrow(attribute: AbilityType, patch: Partial<SavingThrowResponse>) {
    setSavingThrows((current) =>
      current.map((savingThrow) => {
        if (savingThrow.attribute !== attribute) {
          return savingThrow
        }

        const next = { ...savingThrow, ...patch }
        return {
          ...next,
          finalValue:
            next.modifier
            + next.customBonus
            + (next.isProficient ? character.proficiencyBonus : 0),
        }
      }),
    )
  }

  async function saveAll() {
    setIsSaving(true)
    setMessage('')

    try {
      const abilityPayload = abilities.reduce(
        (payload, ability) => ({ ...payload, [lowerFirst(ability.attribute)]: ability.score }),
        {} as Record<string, number>,
      )
      const savingThrowPayload = savingThrows.map((savingThrow) => ({
        attribute: savingThrow.attribute,
        isProficient: savingThrow.isProficient,
        customBonus: savingThrow.customBonus,
      }))

      const [abilityData, savingThrowData] = await Promise.all([
        apiRequest<AbilityScoreResponse[]>(`/api/characters/${character.id}/attributes`, auth.token, {
          method: 'PUT',
          body: JSON.stringify(abilityPayload),
        }),
        apiRequest<SavingThrowResponse[]>(`/api/characters/${character.id}/saving-throws`, auth.token, {
          method: 'PUT',
          body: JSON.stringify(savingThrowPayload),
        }),
      ])

      setAbilities(abilityData)
      setSavingThrows(savingThrowData)
      setMessage('Atributos salvos.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao salvar atributos.')
    } finally {
      setIsSaving(false)
    }
  }

  if (isLoading) {
    return <PanelText>Carregando atributos...</PanelText>
  }

  return (
    <section className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h3 className="text-xl font-semibold">Atributos e testes de resistência</h3>
          <p className="mt-1 text-sm text-slate-600 dark:text-slate-300">
            Modificador = floor((valor - 10) / 2). Proficiência atual: +{character.proficiencyBonus}.
          </p>
        </div>
        {character.canEdit && (
          <button
            className="rounded-md bg-emerald-600 px-4 py-3 text-sm font-semibold text-white disabled:bg-slate-400"
            disabled={isSaving}
            onClick={saveAll}
            type="button"
          >
            {isSaving ? 'Salvando...' : 'Salvar'}
          </button>
        )}
      </div>

      {message && (
        <p className="rounded-md bg-emerald-50 p-3 text-sm text-emerald-800 dark:bg-emerald-950 dark:text-emerald-200">
          {message}
        </p>
      )}

      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
        {abilities.map((ability) => (
          <div
            className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900"
            key={ability.attribute}
          >
            <div className="flex items-center justify-between gap-3">
              <h4 className="font-semibold">{ability.label}</h4>
              <div className="flex items-center gap-2">
                <button
                  className="rounded-md border border-slate-300 p-2 dark:border-slate-700"
                  onClick={() => onRoll({ expression: `1d20${formatSigned(ability.modifier)}`, advantage: false, disadvantage: false, label: ability.label })}
                  type="button"
                  aria-label={`Rolar ${ability.label}`}
                >
                  <Dices size={16} />
                </button>
                <span className="rounded-md bg-slate-100 px-3 py-1 font-mono text-lg font-semibold dark:bg-slate-950">
                  {formatSigned(ability.modifier)}
                </span>
              </div>
            </div>
            <label className="mt-4 block">
              <span className="text-sm text-slate-500 dark:text-slate-400">Valor</span>
              <input
                className="mt-1 min-h-11 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-lg font-semibold text-slate-950 shadow-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 disabled:opacity-70 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
                disabled={!character.canEdit}
                max={30}
                min={1}
                onChange={(event) => updateAbility(ability.attribute, Number(event.target.value))}
                type="number"
                value={ability.score}
              />
            </label>
          </div>
        ))}
      </div>

      <div className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
        <h4 className="text-lg font-semibold">Testes de resistência</h4>
        <div className="mt-4 grid gap-3">
          {savingThrows.map((savingThrow) => (
            <div
              className="grid gap-3 rounded-md border border-slate-200 p-3 dark:border-slate-800 sm:grid-cols-[1fr_auto_auto_auto_auto] sm:items-center"
              key={savingThrow.attribute}
            >
              <div>
                <p className="font-medium">{savingThrow.label}</p>
                <p className="text-sm text-slate-500 dark:text-slate-400">
                  Mod {formatSigned(savingThrow.modifier)}
                </p>
              </div>
              <label className="flex items-center gap-2 text-sm">
                <input
                  checked={savingThrow.isProficient}
                  className="size-4"
                  disabled={!character.canEdit}
                  onChange={(event) => updateSavingThrow(savingThrow.attribute, { isProficient: event.target.checked })}
                  type="checkbox"
                />
                Proficiente
              </label>
              <label className="block text-sm">
                <span className="text-slate-500 dark:text-slate-400">Bônus</span>
                <input
                  className="mt-1 min-h-11 w-24 rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 disabled:opacity-70 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
                  disabled={!character.canEdit}
                  onChange={(event) => updateSavingThrow(savingThrow.attribute, { customBonus: Number(event.target.value) })}
                  type="number"
                  value={savingThrow.customBonus}
                />
              </label>
              <div className="rounded-md bg-slate-100 px-3 py-2 text-center font-mono text-lg font-semibold dark:bg-slate-950">
                {formatSigned(savingThrow.finalValue)}
              </div>
              <button
                className="rounded-md border border-slate-300 p-2 dark:border-slate-700"
                onClick={() => onRoll({ expression: `1d20${formatSigned(savingThrow.finalValue)}`, advantage: false, disadvantage: false, label: `TR ${savingThrow.label}` })}
                type="button"
                aria-label={`Rolar teste de resistência ${savingThrow.label}`}
              >
                <Dices size={17} />
              </button>
            </div>
          ))}
        </div>
      </div>
    </section>
  )
}

function CharacterSkillsSection({
  auth,
  character,
  onRoll,
}: {
  auth: AuthContextValue
  character: CharacterResponse
  onRoll: RollDiceFn
}) {
  const [skills, setSkills] = useState<CharacterSkillResponse[]>([])
  const [message, setMessage] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)

  useEffect(() => {
    apiRequest<CharacterSkillResponse[]>(`/api/characters/${character.id}/skills`, auth.token)
      .then(setSkills)
      .catch((error) => setMessage(error instanceof Error ? error.message : 'Erro ao carregar perícias.'))
      .finally(() => setIsLoading(false))
  }, [auth.token, character.id])

  function updateSkill(skillType: SkillType, patch: Partial<CharacterSkillResponse>) {
    setSkills((current) =>
      current.map((skill) => {
        if (skill.skillType !== skillType) {
          return skill
        }

        const baseValue =
          skill.finalValue
          - skill.customBonus
          - (skill.isProficient ? character.proficiencyBonus : 0)
          - (skill.isExpertise ? character.proficiencyBonus : 0)
        const next = { ...skill, ...patch }

        return {
          ...next,
          finalValue:
            baseValue
            + next.customBonus
            + (next.isProficient ? character.proficiencyBonus : 0)
            + (next.isExpertise ? character.proficiencyBonus : 0),
        }
      }),
    )
  }

  async function saveSkills() {
    setIsSaving(true)
    setMessage('')

    try {
      const payload = skills.map((skill) => ({
        skillType: skill.skillType,
        isProficient: skill.isProficient,
        isExpertise: skill.isExpertise,
        customBonus: skill.customBonus,
      }))

      setSkills(await apiRequest<CharacterSkillResponse[]>(`/api/characters/${character.id}/skills`, auth.token, {
        method: 'PUT',
        body: JSON.stringify(payload),
      }))
      setMessage('Perícias salvas.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao salvar perícias.')
    } finally {
      setIsSaving(false)
    }
  }

  const passivePerception = 10 + (skills.find((skill) => skill.skillType === 'Perception')?.finalValue ?? 0)

  if (isLoading) {
    return <PanelText>Carregando perícias...</PanelText>
  }

  return (
    <section className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h3 className="text-xl font-semibold">Perícias</h3>
          <p className="mt-1 text-sm text-slate-600 dark:text-slate-300">
            Proficiência atual: +{character.proficiencyBonus}. Especialista dobra proficiência.
          </p>
        </div>
        {character.canEdit && (
          <button
            className="rounded-md bg-emerald-600 px-4 py-3 text-sm font-semibold text-white disabled:bg-slate-400"
            disabled={isSaving}
            onClick={saveSkills}
            type="button"
          >
            {isSaving ? 'Salvando...' : 'Salvar'}
          </button>
        )}
      </div>

      <div className="rounded-lg border border-emerald-200 bg-emerald-50 p-5 dark:border-emerald-900 dark:bg-emerald-950">
        <p className="text-sm font-medium text-emerald-800 dark:text-emerald-200">Percepção Passiva</p>
        <p className="mt-1 text-3xl font-semibold text-emerald-950 dark:text-emerald-100">{passivePerception}</p>
      </div>

      {message && (
        <p className="rounded-md bg-emerald-50 p-3 text-sm text-emerald-800 dark:bg-emerald-950 dark:text-emerald-200">
          {message}
        </p>
      )}

      <div className="grid gap-3">
        {skills.map((skill) => (
          <div
            className="grid gap-3 rounded-lg border border-slate-200 bg-white p-4 dark:border-slate-800 dark:bg-slate-900 lg:grid-cols-[1fr_auto_auto_auto_auto_auto] lg:items-center"
            key={skill.skillType}
          >
            <div className="min-w-0">
              <div className="flex items-center gap-2">
                <p className="font-semibold">{skill.label}</p>
                <span className="rounded-full bg-slate-100 px-2 py-0.5 text-xs text-slate-600 dark:bg-slate-950 dark:text-slate-300">
                  {skill.baseAttributeLabel}
                </span>
              </div>
              <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">{skill.skillType}</p>
            </div>
            <label className="flex items-center gap-2 text-sm">
              <input
                checked={skill.isProficient}
                className="size-4"
                disabled={!character.canEdit}
                onChange={(event) => updateSkill(skill.skillType, { isProficient: event.target.checked })}
                type="checkbox"
              />
              Proficiente
            </label>
            <label className="flex items-center gap-2 text-sm">
              <input
                checked={skill.isExpertise}
                className="size-4"
                disabled={!character.canEdit}
                onChange={(event) => updateSkill(skill.skillType, { isExpertise: event.target.checked })}
                type="checkbox"
              />
              Especialista
            </label>
            <label className="block text-sm">
              <span className="text-slate-500 dark:text-slate-400">Bônus</span>
              <input
                className="mt-1 min-h-11 w-24 rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 disabled:opacity-70 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
                disabled={!character.canEdit}
                onChange={(event) => updateSkill(skill.skillType, { customBonus: Number(event.target.value) })}
                type="number"
                value={skill.customBonus}
              />
            </label>
            <div className="rounded-md bg-slate-100 px-3 py-2 text-center font-mono text-lg font-semibold dark:bg-slate-950">
              {formatSigned(skill.finalValue)}
            </div>
            <button
              className="rounded-md border border-slate-300 p-2 dark:border-slate-700"
              onClick={() => onRoll({ expression: `1d20${formatSigned(skill.finalValue)}`, advantage: false, disadvantage: false, label: skill.label })}
              type="button"
              aria-label={`Rolar ${skill.label}`}
            >
              <Dices size={17} />
            </button>
          </div>
        ))}
      </div>
    </section>
  )
}

function CharacterCombatSection({
  auth,
  character,
  onRoll,
  restVersion,
}: {
  auth: AuthContextValue
  character: CharacterResponse
  onRoll: RollDiceFn
  restVersion: number
}) {
  const [combat, setCombat] = useState<CharacterCombatResponse | null>(null)
  const [attacks, setAttacks] = useState<CharacterAttackResponse[]>([])
  const [conditions, setConditions] = useState<CharacterConditionResponse[]>([])
  const [attackDraft, setAttackDraft] = useState<CharacterAttackResponse>(emptyAttack())
  const [editingAttackId, setEditingAttackId] = useState<string | null>(null)
  const [quickAmount, setQuickAmount] = useState(1)
  const [message, setMessage] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)

  useEffect(() => {
    Promise.all([
      apiRequest<CharacterCombatResponse>(`/api/characters/${character.id}/combat`, auth.token),
      apiRequest<CharacterAttackResponse[]>(`/api/characters/${character.id}/attacks`, auth.token),
      apiRequest<CharacterConditionResponse[]>(`/api/characters/${character.id}/conditions`, auth.token),
    ])
      .then(([combatData, attackData, conditionData]) => {
        setCombat(combatData)
        setAttacks(attackData)
        setConditions(conditionData)
      })
      .catch((error) => setMessage(error instanceof Error ? error.message : 'Erro ao carregar combate.'))
      .finally(() => setIsLoading(false))
  }, [auth.token, character.id, restVersion])

  function patchCombat(patch: Partial<CharacterCombatResponse>) {
    setCombat((current) => (current ? { ...current, ...patch } : current))
  }

  async function saveCombat(nextCombat = combat, successMessage = 'Combate salvo.') {
    if (!nextCombat) {
      return
    }

    setIsSaving(true)
    setMessage('')

    try {
      setCombat(await apiRequest<CharacterCombatResponse>(`/api/characters/${character.id}/combat`, auth.token, {
        method: 'PUT',
        body: JSON.stringify(nextCombat),
      }))
      setMessage(successMessage)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao salvar combate.')
    } finally {
      setIsSaving(false)
    }
  }

  function applyDamage() {
    if (!combat) {
      return
    }

    const amount = Math.max(0, quickAmount)
    const tempDamage = Math.min(combat.temporaryHitPoints, amount)
    const remainingDamage = amount - tempDamage
    const nextCombat = {
      ...combat,
      temporaryHitPoints: combat.temporaryHitPoints - tempDamage,
      currentHitPoints: Math.max(0, combat.currentHitPoints - remainingDamage),
    }
    setCombat(nextCombat)
    void saveCombat(nextCombat, 'Dano aplicado.')
  }

  function heal() {
    if (!combat) {
      return
    }

    const nextCombat = {
      ...combat,
      currentHitPoints: Math.min(combat.maxHitPoints, combat.currentHitPoints + Math.max(0, quickAmount)),
    }
    setCombat(nextCombat)
    void saveCombat(nextCombat, 'Cura aplicada.')
  }

  function addTemporaryHitPoints() {
    if (!combat) {
      return
    }

    const nextCombat = {
      ...combat,
      temporaryHitPoints: Math.max(combat.temporaryHitPoints, Math.max(0, quickAmount)),
    }
    setCombat(nextCombat)
    void saveCombat(nextCombat, 'Vida temporária ajustada.')
  }

  function patchAttack(patch: Partial<CharacterAttackResponse>) {
    setAttackDraft((current) => ({ ...current, ...patch }))
  }

  function editAttack(attack: CharacterAttackResponse) {
    setEditingAttackId(attack.id)
    setAttackDraft(attack)
  }

  async function saveAttack(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setIsSaving(true)
    setMessage('')

    const payload = {
      name: attackDraft.name,
      attackBonus: attackDraft.attackBonus,
      damage: attackDraft.damage,
      damageType: attackDraft.damageType,
      range: attackDraft.range,
      usesAttribute: attackDraft.usesAttribute || null,
      notes: attackDraft.notes,
    }

    try {
      const saved = await apiRequest<CharacterAttackResponse>(
        editingAttackId
          ? `/api/characters/${character.id}/attacks/${editingAttackId}`
          : `/api/characters/${character.id}/attacks`,
        auth.token,
        {
          method: editingAttackId ? 'PUT' : 'POST',
          body: JSON.stringify(payload),
        },
      )

      setAttacks((current) =>
        editingAttackId
          ? current.map((attack) => (attack.id === saved.id ? saved : attack))
          : [...current, saved].sort((left, right) => left.name.localeCompare(right.name)),
      )
      setAttackDraft(emptyAttack())
      setEditingAttackId(null)
      setMessage('Ataque salvo.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao salvar ataque.')
    } finally {
      setIsSaving(false)
    }
  }

  async function deleteAttack(attackId: string) {
    setIsSaving(true)
    setMessage('')

    try {
      await apiRequest<null>(`/api/characters/${character.id}/attacks/${attackId}`, auth.token, {
        method: 'DELETE',
      })
      setAttacks((current) => current.filter((attack) => attack.id !== attackId))
      setMessage('Ataque excluído.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao excluir ataque.')
    } finally {
      setIsSaving(false)
    }
  }

  function patchCondition(conditionType: ConditionType, patch: Partial<CharacterConditionResponse>) {
    setConditions((current) =>
      current.map((condition) =>
        condition.conditionType === conditionType ? { ...condition, ...patch } : condition,
      ),
    )
  }

  async function saveConditions() {
    setIsSaving(true)
    setMessage('')

    try {
      setConditions(await apiRequest<CharacterConditionResponse[]>(`/api/characters/${character.id}/conditions`, auth.token, {
        method: 'PUT',
        body: JSON.stringify(conditions.map(({ conditionType, name, description, isActive, notes }) => ({
          conditionType,
          name,
          description,
          isActive,
          notes,
        }))),
      }))
      setMessage('Condições salvas.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao salvar condições.')
    } finally {
      setIsSaving(false)
    }
  }

  if (isLoading || !combat) {
    return <PanelText>Carregando combate...</PanelText>
  }

  return (
    <section className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h3 className="text-xl font-semibold">Combate</h3>
          <p className="mt-1 text-sm text-slate-600 dark:text-slate-300">
            Vida, defesa, ataques e condições ativas da ficha.
          </p>
        </div>
        {character.canEdit && (
          <button
            className="rounded-md bg-emerald-600 px-4 py-3 text-sm font-semibold text-white disabled:bg-slate-400"
            disabled={isSaving}
            onClick={() => saveCombat()}
            type="button"
          >
            {isSaving ? 'Salvando...' : 'Salvar combate'}
          </button>
        )}
      </div>

      {message && (
        <p className="rounded-md bg-emerald-50 p-3 text-sm text-emerald-800 dark:bg-emerald-950 dark:text-emerald-200">
          {message}
        </p>
      )}

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <CombatNumber label="CA" disabled={!character.canEdit} onChange={(value) => patchCombat({ armorClass: value })} value={combat.armorClass} />
        <CombatNumber label="Iniciativa" disabled={!character.canEdit} onChange={(value) => patchCombat({ initiative: value })} value={combat.initiative} />
        <CombatNumber label="Deslocamento" disabled={!character.canEdit} onChange={(value) => patchCombat({ speed: value })} value={combat.speed} />
        <CombatText label="Dados de vida" disabled={!character.canEdit} onChange={(value) => patchCombat({ availableHitDice: value })} value={combat.availableHitDice} />
      </div>

      <div className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
        <div className="grid gap-4 md:grid-cols-3">
          <CombatNumber label="PV atual" disabled={!character.canEdit} min={0} onChange={(value) => patchCombat({ currentHitPoints: value })} value={combat.currentHitPoints} />
          <CombatNumber label="PV máximo" disabled={!character.canEdit} min={0} onChange={(value) => patchCombat({ maxHitPoints: value })} value={combat.maxHitPoints} />
          <CombatNumber label="PV temporário" disabled={!character.canEdit} min={0} onChange={(value) => patchCombat({ temporaryHitPoints: value })} value={combat.temporaryHitPoints} />
        </div>
        <div className="mt-4 h-3 overflow-hidden rounded-full bg-slate-100 dark:bg-slate-950">
          <div
            className="h-full bg-emerald-600"
            style={{ width: `${Math.min(100, Math.round((combat.currentHitPoints / Math.max(1, combat.maxHitPoints)) * 100))}%` }}
          />
        </div>
        {character.canEdit && (
          <div className="mt-5 grid gap-3 sm:grid-cols-[160px_1fr_1fr_1fr]">
            <CombatNumber label="Valor rápido" min={0} onChange={setQuickAmount} value={quickAmount} />
            <button className="rounded-md border border-red-300 px-3 py-2 text-sm font-semibold text-red-700 dark:border-red-900 dark:text-red-300" disabled={isSaving} onClick={applyDamage} type="button">
              Receber dano
            </button>
            <button className="rounded-md border border-emerald-300 px-3 py-2 text-sm font-semibold text-emerald-700 dark:border-emerald-900 dark:text-emerald-300" disabled={isSaving} onClick={heal} type="button">
              Curar
            </button>
            <button className="rounded-md border border-sky-300 px-3 py-2 text-sm font-semibold text-sky-700 dark:border-sky-900 dark:text-sky-300" disabled={isSaving} onClick={addTemporaryHitPoints} type="button">
              Adicionar vida temporária
            </button>
          </div>
        )}
      </div>

      <div className="grid gap-6 xl:grid-cols-[1fr_1.2fr]">
        <div className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
          <div className="flex items-center justify-between gap-3">
            <h4 className="text-lg font-semibold">Ataques</h4>
            {editingAttackId && (
              <button className="text-sm font-medium text-slate-500 dark:text-slate-400" onClick={() => { setEditingAttackId(null); setAttackDraft(emptyAttack()) }} type="button">
                Cancelar edição
              </button>
            )}
          </div>

          {character.canEdit && (
            <form className="mt-4 grid gap-3" onSubmit={saveAttack}>
              <CombatText label="Nome" onChange={(value) => patchAttack({ name: value })} value={attackDraft.name} />
              <div className="grid gap-3 sm:grid-cols-2">
                <CombatNumber label="Bônus de ataque" onChange={(value) => patchAttack({ attackBonus: value })} value={attackDraft.attackBonus} />
                <CombatText label="Dano" onChange={(value) => patchAttack({ damage: value })} value={attackDraft.damage} />
                <CombatText label="Tipo de dano" onChange={(value) => patchAttack({ damageType: value })} value={attackDraft.damageType} />
                <CombatText label="Alcance" onChange={(value) => patchAttack({ range: value })} value={attackDraft.range} />
              </div>
              <label className="block">
                <span className="text-sm font-medium text-slate-700 dark:text-slate-200">Atributo usado</span>
                <select
                  className="mt-1 min-h-11 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
                  onChange={(event) => patchAttack({ usesAttribute: (event.target.value || null) as AbilityType | null })}
                  value={attackDraft.usesAttribute ?? ''}
                >
                  <option value="">Nenhum</option>
                  <option value="Strength">Força</option>
                  <option value="Dexterity">Destreza</option>
                  <option value="Constitution">Constituição</option>
                  <option value="Intelligence">Inteligência</option>
                  <option value="Wisdom">Sabedoria</option>
                  <option value="Charisma">Carisma</option>
                </select>
              </label>
              <CombatTextArea label="Notas" onChange={(value) => patchAttack({ notes: value })} value={attackDraft.notes} />
              <button className="inline-flex items-center justify-center gap-2 rounded-md bg-emerald-600 px-4 py-3 text-sm font-semibold text-white disabled:bg-slate-400" disabled={isSaving} type="submit">
                <Plus size={18} />
                {editingAttackId ? 'Salvar ataque' : 'Adicionar ataque'}
              </button>
            </form>
          )}

          <div className="mt-5 grid gap-3">
            {attacks.length === 0 ? (
              <p className="text-sm text-slate-500 dark:text-slate-400">Sem ataques cadastrados.</p>
            ) : attacks.map((attack) => (
              <div className="rounded-md border border-slate-200 p-4 dark:border-slate-800" key={attack.id}>
                <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
                  <div>
                    <h5 className="font-semibold">{attack.name}</h5>
                    <p className="mt-1 text-sm text-slate-600 dark:text-slate-300">
                      Ataque {formatSigned(attack.attackBonus)} · {attack.damage || '-'} {attack.damageType || ''} · {attack.range || '-'}
                    </p>
                    {attack.usesAttributeLabel && <p className="mt-1 text-xs text-slate-500 dark:text-slate-400">Atributo: {attack.usesAttributeLabel}</p>}
                    {attack.notes && <p className="mt-2 whitespace-pre-wrap text-sm text-slate-500 dark:text-slate-400">{attack.notes}</p>}
                  </div>
                  <div className="flex gap-2">
                    <button
                      className="rounded-md border border-slate-300 p-2 dark:border-slate-700"
                      onClick={() => onRoll({ expression: `1d20${formatSigned(attack.attackBonus)}`, advantage: false, disadvantage: false, label: `Ataque: ${attack.name}` })}
                      type="button"
                      aria-label={`Rolar ataque ${attack.name}`}
                    >
                      <Dices size={17} />
                    </button>
                    {character.canEdit && (
                      <>
                      <button className="rounded-md border border-slate-300 p-2 dark:border-slate-700" onClick={() => editAttack(attack)} type="button" aria-label="Editar ataque">
                        <Edit size={17} />
                      </button>
                      <button className="rounded-md border border-red-300 p-2 text-red-700 dark:border-red-900 dark:text-red-300" onClick={() => deleteAttack(attack.id)} type="button" aria-label="Excluir ataque">
                        <Trash2 size={17} />
                      </button>
                      </>
                    )}
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
          <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
            <h4 className="text-lg font-semibold">Condições</h4>
            {character.canEdit && (
              <button className="rounded-md bg-emerald-600 px-4 py-2 text-sm font-semibold text-white disabled:bg-slate-400" disabled={isSaving} onClick={saveConditions} type="button">
                Salvar condições
              </button>
            )}
          </div>
          <div className="mt-4 grid gap-3">
            {conditions.map((condition) => (
              <div className={`rounded-md border p-4 ${condition.isActive ? 'border-amber-300 bg-amber-50 dark:border-amber-900 dark:bg-amber-950' : 'border-slate-200 dark:border-slate-800'}`} key={condition.conditionType}>
                <label className="flex items-start gap-3">
                  <input
                    checked={condition.isActive}
                    className="mt-1 size-4"
                    disabled={!character.canEdit}
                    onChange={(event) => patchCondition(condition.conditionType, { isActive: event.target.checked })}
                    type="checkbox"
                  />
                  <span className="min-w-0">
                    <span className="block font-semibold">{condition.name}</span>
                    <span className="mt-1 block text-sm text-slate-600 dark:text-slate-300">{condition.description}</span>
                  </span>
                </label>
                {character.canEdit ? (
                  <div className="mt-3 grid gap-3">
                    <CombatText label="Nome editável" onChange={(value) => patchCondition(condition.conditionType, { name: value })} value={condition.name} />
                    <CombatTextArea label="Descrição editável" onChange={(value) => patchCondition(condition.conditionType, { description: value })} value={condition.description} />
                    <CombatTextArea label="Notas" onChange={(value) => patchCondition(condition.conditionType, { notes: value })} value={condition.notes} />
                  </div>
                ) : condition.notes ? (
                  <p className="mt-3 whitespace-pre-wrap text-sm text-slate-500 dark:text-slate-400">{condition.notes}</p>
                ) : null}
              </div>
            ))}
          </div>
        </div>
      </div>
    </section>
  )
}

const noteCategories = ['Diário', 'NPCs', 'Missões', 'Lugares', 'Segredos', 'Itens', 'Teorias', 'Sessões', 'Outros']

function CharacterNotebookSection({
  auth,
  character,
}: {
  auth: AuthContextValue
  character: CharacterResponse
}) {
  const [notes, setNotes] = useState<CharacterNoteResponse[]>([])
  const [selectedNoteId, setSelectedNoteId] = useState<string | null>(null)
  const [draft, setDraft] = useState<CharacterNoteResponse>(emptyNote(character.id))
  const [search, setSearch] = useState('')
  const [message, setMessage] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)

  useEffect(() => {
    const query = search.trim() ? `?search=${encodeURIComponent(search.trim())}` : ''
    apiRequest<CharacterNoteResponse[]>(`/api/characters/${character.id}/notes${query}`, auth.token)
      .then((data) => {
        setNotes(data)
        if (selectedNoteId && !data.some((note) => note.id === selectedNoteId)) {
          setSelectedNoteId(null)
          setDraft(emptyNote(character.id))
        }
      })
      .catch((error) => setMessage(error instanceof Error ? error.message : 'Erro ao carregar notas.'))
      .finally(() => setIsLoading(false))
  }, [auth.token, character.id, search, selectedNoteId])

  function patchDraft(patch: Partial<CharacterNoteResponse>) {
    setDraft((current) => {
      const next = { ...current, ...patch }
      return next.isPrivate ? { ...next, isVisibleToMaster: false } : next
    })
  }

  function selectNote(note: CharacterNoteResponse) {
    setSelectedNoteId(note.id)
    setDraft(note)
    setMessage('')
  }

  function newNote() {
    setSelectedNoteId(null)
    setDraft(emptyNote(character.id))
    setMessage('')
  }

  async function saveNote(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setIsSaving(true)
    setMessage('')

    const payload = {
      title: draft.title,
      content: draft.content,
      category: draft.category,
      tags: draft.tags,
      isPrivate: draft.isPrivate,
      isVisibleToMaster: draft.isPrivate ? false : draft.isVisibleToMaster,
    }

    try {
      const saved = await apiRequest<CharacterNoteResponse>(
        selectedNoteId
          ? `/api/characters/${character.id}/notes/${selectedNoteId}`
          : `/api/characters/${character.id}/notes`,
        auth.token,
        {
          method: selectedNoteId ? 'PUT' : 'POST',
          body: JSON.stringify(payload),
        },
      )

      setNotes((current) =>
        selectedNoteId
          ? current.map((note) => (note.id === saved.id ? saved : note))
          : [saved, ...current],
      )
      setSelectedNoteId(saved.id)
      setDraft(saved)
      setMessage('Nota salva.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao salvar nota.')
    } finally {
      setIsSaving(false)
    }
  }

  async function deleteNote() {
    if (!selectedNoteId) {
      return
    }

    setIsSaving(true)
    setMessage('')

    try {
      await apiRequest<null>(`/api/characters/${character.id}/notes/${selectedNoteId}`, auth.token, {
        method: 'DELETE',
      })
      setNotes((current) => current.filter((note) => note.id !== selectedNoteId))
      newNote()
      setMessage('Nota excluída.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao excluir nota.')
    } finally {
      setIsSaving(false)
    }
  }

  return (
    <section className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h3 className="text-xl font-semibold">Notebook</h3>
          <p className="mt-1 text-sm text-slate-600 dark:text-slate-300">
            Diário, pistas, NPCs, lugares e ideias do personagem. Nota privada só você vê; nota visível para o mestre pode ser lida pelo mestre da campanha.
          </p>
        </div>
        {character.canEdit && (
          <button
            className="inline-flex items-center justify-center gap-2 rounded-md bg-emerald-600 px-4 py-3 text-sm font-semibold text-white"
            onClick={newNote}
            type="button"
          >
            <Plus size={18} />
            Nova nota
          </button>
        )}
      </div>

      {message && (
        <p className="rounded-md bg-emerald-50 p-3 text-sm text-emerald-800 dark:bg-emerald-950 dark:text-emerald-200">
          {message}
        </p>
      )}

      <label className="block">
        <span className="text-sm font-medium text-slate-700 dark:text-slate-200">Buscar</span>
        <input
          className="mt-1 min-h-11 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
          onChange={(event) => setSearch(event.target.value)}
          placeholder="Título, conteúdo, categoria ou tags"
          value={search}
        />
      </label>

      <div className="grid gap-6 xl:grid-cols-[320px_1fr]">
        <div className="rounded-lg border border-slate-200 bg-white p-4 dark:border-slate-800 dark:bg-slate-900">
          <h4 className="font-semibold">Notas</h4>
          <div className="mt-4 grid gap-2">
            {isLoading ? (
              <p className="text-sm text-slate-500 dark:text-slate-400">Carregando notas...</p>
            ) : notes.length === 0 ? (
              <p className="text-sm text-slate-500 dark:text-slate-400">Nenhuma nota encontrada.</p>
            ) : notes.map((note) => (
              <button
                className={`rounded-md border p-3 text-left ${selectedNoteId === note.id ? 'border-emerald-400 bg-emerald-50 dark:border-emerald-800 dark:bg-emerald-950' : 'border-slate-200 dark:border-slate-800'}`}
                key={note.id}
                onClick={() => selectNote(note)}
                type="button"
              >
                <span className="block truncate font-semibold">{note.title}</span>
                <span className="mt-1 block text-xs text-slate-500 dark:text-slate-400">{note.category || 'Outros'}</span>
                <span className="mt-2 flex flex-wrap gap-2 text-xs">
                  {note.isPrivate && <span className="rounded-full bg-slate-100 px-2 py-0.5 dark:bg-slate-800">Privada</span>}
                  {note.isVisibleToMaster && <span className="rounded-full bg-emerald-100 px-2 py-0.5 text-emerald-800 dark:bg-emerald-900 dark:text-emerald-100">Mestre</span>}
                </span>
              </button>
            ))}
          </div>
        </div>

        <div className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
          {character.canEdit ? (
            <form className="grid gap-4" onSubmit={saveNote}>
              <CombatText label="Título" onChange={(value) => patchDraft({ title: value })} value={draft.title} />
              <label className="block">
                <span className="text-sm font-medium text-slate-700 dark:text-slate-200">Categoria</span>
                <select
                  className="mt-1 min-h-11 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
                  onChange={(event) => patchDraft({ category: event.target.value })}
                  value={draft.category}
                >
                  {noteCategories.map((category) => (
                    <option key={category} value={category}>{category}</option>
                  ))}
                </select>
              </label>
              <CombatText label="Tags" onChange={(value) => patchDraft({ tags: value })} value={draft.tags} />
              <CombatTextArea label="Conteúdo" onChange={(value) => patchDraft({ content: value })} value={draft.content} />
              <div className="grid gap-3 sm:grid-cols-2">
                <label className="flex items-center gap-2 text-sm">
                  <input
                    checked={draft.isPrivate}
                    className="size-4"
                    onChange={(event) => patchDraft({ isPrivate: event.target.checked })}
                    type="checkbox"
                  />
                  Privada
                </label>
                <label className="flex items-center gap-2 text-sm">
                  <input
                    checked={draft.isVisibleToMaster}
                    className="size-4"
                    disabled={draft.isPrivate}
                    onChange={(event) => patchDraft({ isVisibleToMaster: event.target.checked })}
                    type="checkbox"
                  />
                  Visível para mestre
                </label>
              </div>
              <div className="flex flex-col gap-3 sm:flex-row sm:justify-between">
                {selectedNoteId ? (
                  <button
                    className="inline-flex items-center justify-center gap-2 rounded-md border border-red-300 px-4 py-3 text-sm font-semibold text-red-700 dark:border-red-900 dark:text-red-300"
                    disabled={isSaving}
                    onClick={deleteNote}
                    type="button"
                  >
                    <Trash2 size={18} />
                    Excluir
                  </button>
                ) : <span />}
                <button
                  className="inline-flex items-center justify-center gap-2 rounded-md bg-emerald-600 px-4 py-3 text-sm font-semibold text-white disabled:bg-slate-400"
                  disabled={isSaving}
                  type="submit"
                >
                  <BookOpen size={18} />
                  {isSaving ? 'Salvando...' : 'Salvar nota'}
                </button>
              </div>
            </form>
          ) : selectedNoteId ? (
            <article>
              <div className="flex flex-wrap items-center gap-2">
                <h4 className="text-xl font-semibold">{draft.title}</h4>
                <span className="rounded-full bg-slate-100 px-2 py-0.5 text-xs dark:bg-slate-800">{draft.category}</span>
              </div>
              {draft.tags && <p className="mt-2 text-sm text-slate-500 dark:text-slate-400">{draft.tags}</p>}
              <p className="mt-4 whitespace-pre-wrap text-sm leading-6 text-slate-700 dark:text-slate-200">{draft.content}</p>
            </article>
          ) : (
            <p className="text-sm text-slate-500 dark:text-slate-400">Selecione nota para ler.</p>
          )}
        </div>
      </div>
    </section>
  )
}

function emptyNote(characterId: string): CharacterNoteResponse {
  return {
    id: '',
    characterId,
    title: '',
    content: '',
    category: 'Diário',
    tags: '',
    isPrivate: false,
    isVisibleToMaster: false,
    createdAt: '',
    updatedAt: null,
    canEdit: true,
  }
}

function emptyCharacterFeature(): CharacterFeaturePayload {
  return {
    featureId: null,
    customName: '',
    customDescription: '',
    maxUses: 0,
    currentUses: 0,
    recoveryType: 'Manual',
    notes: '',
  }
}

function CharacterFeaturesSection({
  auth,
  character,
  restVersion,
}: {
  auth: AuthContextValue
  character: CharacterResponse
  restVersion: number
}) {
  const [characterFeatures, setCharacterFeatures] = useState<CharacterFeatureResponse[]>([])
  const [search, setSearch] = useState('')
  const [libraryResults, setLibraryResults] = useState<FeatureResponse[]>([])
  const [manualDraft, setManualDraft] = useState<CharacterFeaturePayload>(emptyCharacterFeature())
  const [message, setMessage] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)

  useEffect(() => {
    loadCharacterFeatures()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [auth.token, character.id, restVersion])

  async function loadCharacterFeatures() {
    setIsLoading(true)
    setMessage('')
    try {
      setCharacterFeatures(await apiRequest<CharacterFeatureResponse[]>(`/api/characters/${character.id}/features`, auth.token))
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao carregar talentos do personagem.')
    } finally {
      setIsLoading(false)
    }
  }

  async function searchLibrary(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setMessage('')
    try {
      const query = new URLSearchParams({ name: search, page: '1', pageSize: '8' })
      const result = await apiRequest<PagedResponse<FeatureResponse>>(`/api/features?${query.toString()}`, auth.token)
      setLibraryResults(result.items)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao buscar biblioteca.')
    }
  }

  async function addFromLibrary(feature: FeatureResponse) {
    setIsSaving(true)
    setMessage('')
    try {
      const added = await apiRequest<CharacterFeatureResponse>(`/api/characters/${character.id}/features`, auth.token, {
        method: 'POST',
        body: JSON.stringify({
          featureId: feature.id,
          customName: '',
          customDescription: '',
          maxUses: 0,
          currentUses: 0,
          recoveryType: 'Manual',
          notes: '',
        }),
      })
      setCharacterFeatures((current) => [...current, added].sort(compareCharacterFeatures))
      setLibraryResults((current) => current.filter((item) => item.id !== feature.id))
      setMessage('Talento/característica adicionado.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao adicionar talento.')
    } finally {
      setIsSaving(false)
    }
  }

  async function addManualFeature(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setIsSaving(true)
    setMessage('')
    try {
      const added = await apiRequest<CharacterFeatureResponse>(`/api/characters/${character.id}/features`, auth.token, {
        method: 'POST',
        body: JSON.stringify(manualDraft),
      })
      setCharacterFeatures((current) => [...current, added].sort(compareCharacterFeatures))
      setManualDraft(emptyCharacterFeature())
      setMessage('Característica manual adicionada.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao adicionar característica manual.')
    } finally {
      setIsSaving(false)
    }
  }

  async function updateCharacterFeature(feature: CharacterFeatureResponse, patch: Partial<CharacterFeatureResponse>) {
    const next = { ...feature, ...patch }
    setCharacterFeatures((current) => current.map((item) => (item.id === feature.id ? next : item)))
    setMessage('')

    try {
      const saved = await apiRequest<CharacterFeatureResponse>(
        `/api/characters/${character.id}/features/${feature.id}`,
        auth.token,
        {
          method: 'PUT',
          body: JSON.stringify({
            featureId: next.featureId ?? null,
            customName: next.customName,
            customDescription: next.customDescription,
            maxUses: next.maxUses,
            currentUses: next.currentUses,
            recoveryType: next.recoveryType,
            notes: next.notes,
          }),
        },
      )
      setCharacterFeatures((current) => current.map((item) => (item.id === saved.id ? saved : item)))
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao salvar talento.')
      await loadCharacterFeatures()
    }
  }

  async function removeFeature(featureId: string) {
    setIsSaving(true)
    setMessage('')
    try {
      await apiRequest<null>(`/api/characters/${character.id}/features/${featureId}`, auth.token, { method: 'DELETE' })
      setCharacterFeatures((current) => current.filter((item) => item.id !== featureId))
      setMessage('Talento/característica removido.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao remover talento.')
    } finally {
      setIsSaving(false)
    }
  }

  function patchManual(patch: Partial<CharacterFeaturePayload>) {
    setManualDraft((current) => ({ ...current, ...patch }))
  }

  const limitedCount = characterFeatures.filter((feature) => feature.maxUses > 0).length

  if (isLoading) {
    return <PanelText>Carregando talentos...</PanelText>
  }

  return (
    <section className="space-y-4">
      {message && <p className="rounded-md bg-emerald-50 p-3 text-sm text-emerald-800 dark:bg-emerald-950 dark:text-emerald-200">{message}</p>}

      <div className="grid gap-4 lg:grid-cols-[minmax(0,1fr)_340px]">
        <div className="space-y-4">
          <div className="grid gap-3 sm:grid-cols-2">
            <StatPill label="Total" value={characterFeatures.length.toString()} />
            <StatPill label="Com usos" value={limitedCount.toString()} />
          </div>

          {character.canEdit && (
            <form className="rounded-lg border border-slate-200 bg-white p-4 dark:border-slate-800 dark:bg-slate-900" onSubmit={searchLibrary}>
              <h3 className="text-lg font-semibold">Adicionar da biblioteca</h3>
              <div className="mt-3 grid gap-3 sm:grid-cols-[1fr_auto]">
                <input
                  className="min-h-11 rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
                  onChange={(event) => setSearch(event.target.value)}
                  placeholder="Buscar por nome"
                  value={search}
                />
                <button className="rounded-md border border-slate-300 px-4 py-2.5 text-sm font-semibold text-slate-700 dark:border-slate-700 dark:text-slate-200" type="submit">
                  Buscar
                </button>
              </div>
              {libraryResults.length > 0 && (
                <div className="mt-3 grid gap-2">
                  {libraryResults.map((feature) => (
                    <div className="flex items-start justify-between gap-3 rounded-md border border-slate-200 p-3 dark:border-slate-800" key={feature.id}>
                      <div>
                        <p className="font-semibold">{feature.name}</p>
                        <p className="text-xs text-slate-500 dark:text-slate-400">{featureTypeLabel(feature.type)} - {feature.source || 'Sem fonte'}</p>
                      </div>
                      <button className="rounded-md bg-emerald-600 px-3 py-2 text-xs font-semibold text-white disabled:bg-slate-400" disabled={isSaving} onClick={() => addFromLibrary(feature)} type="button">
                        Adicionar
                      </button>
                    </div>
                  ))}
                </div>
              )}
            </form>
          )}

          {characterFeatures.length === 0 ? (
            <PanelText>Nenhum talento ou característica vinculado ainda.</PanelText>
          ) : (
            <div className="grid gap-3">
              {characterFeatures.map((feature) => (
                <CharacterFeatureCard
                  canEdit={character.canEdit}
                  feature={feature}
                  key={feature.id}
                  onRemove={removeFeature}
                  onUpdate={updateCharacterFeature}
                />
              ))}
            </div>
          )}
        </div>

        {character.canEdit && (
          <aside className="rounded-lg border border-slate-200 bg-white p-4 dark:border-slate-800 dark:bg-slate-900">
            <h3 className="text-lg font-semibold">Característica manual</h3>
            <form className="mt-3 grid gap-3" onSubmit={addManualFeature}>
              <TextField label="Nome" onChange={(value) => patchManual({ customName: value })} required value={manualDraft.customName} />
              <TextAreaField label="Descrição" onChange={(value) => patchManual({ customDescription: value })} value={manualDraft.customDescription} />
              <div className="grid gap-3 sm:grid-cols-2">
                <NumberField label="Usos máximos" min={0} onChange={(value) => patchManual({ maxUses: value, currentUses: Math.min(manualDraft.currentUses, value) })} value={manualDraft.maxUses} />
                <NumberField label="Usos atuais" min={0} onChange={(value) => patchManual({ currentUses: value })} value={manualDraft.currentUses} />
              </div>
              <SimpleSelect label="Recuperação" onChange={(value) => patchManual({ recoveryType: value as RecoveryType })} options={recoveryTypeOptions} value={manualDraft.recoveryType} />
              <TextAreaField label="Notas" onChange={(value) => patchManual({ notes: value })} value={manualDraft.notes} />
              <button className="inline-flex items-center justify-center gap-2 rounded-md bg-emerald-600 px-4 py-3 text-sm font-semibold text-white disabled:bg-slate-400" disabled={isSaving} type="submit">
                <Plus size={18} />
                Adicionar manual
              </button>
            </form>
          </aside>
        )}
      </div>
    </section>
  )
}

function CharacterFeatureCard({
  canEdit,
  feature,
  onRemove,
  onUpdate,
}: {
  canEdit: boolean
  feature: CharacterFeatureResponse
  onRemove: (featureId: string) => void
  onUpdate: (feature: CharacterFeatureResponse, patch: Partial<CharacterFeatureResponse>) => void
}) {
  const canDecrease = canEdit && feature.currentUses > 0
  const canIncrease = canEdit && feature.maxUses > 0 && feature.currentUses < feature.maxUses

  return (
    <div className="rounded-lg border border-slate-200 bg-white p-4 dark:border-slate-800 dark:bg-slate-900">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
        <div>
          <div className="flex flex-wrap items-center gap-2">
            <h3 className="text-lg font-semibold">{feature.name}</h3>
            <span className="rounded-full bg-slate-100 px-2.5 py-1 text-xs font-semibold text-slate-700 dark:bg-slate-800 dark:text-slate-200">
              {feature.typeLabel ?? 'Manual'}
            </span>
          </div>
          <p className="mt-1 text-xs text-slate-500 dark:text-slate-400">
            {feature.source || 'Característica manual'} - {feature.recoveryTypeLabel}
          </p>
        </div>
        {canEdit && (
          <button className="self-start rounded-md border border-red-300 p-2 text-red-700 dark:border-red-900 dark:text-red-300" onClick={() => onRemove(feature.id)} type="button" aria-label="Remover talento">
            <Trash2 size={16} />
          </button>
        )}
      </div>

      <p className="mt-3 whitespace-pre-wrap text-sm leading-6 text-slate-600 dark:text-slate-300">
        {feature.description || 'Sem descrição.'}
      </p>
      {feature.prerequisites && <p className="mt-2 text-xs text-slate-500 dark:text-slate-400">Pré-requisitos: {feature.prerequisites}</p>}

      <div className="mt-4 grid gap-3 md:grid-cols-[1fr_1fr_auto] md:items-end">
        <NumberField
          label="Usos máximos"
          min={0}
          onChange={(value) => onUpdate(feature, { maxUses: value, currentUses: Math.min(feature.currentUses, value) })}
          value={feature.maxUses}
          disabled={!canEdit}
        />
        <NumberField
          label="Usos atuais"
          min={0}
          onChange={(value) => onUpdate(feature, { currentUses: value })}
          value={feature.currentUses}
          disabled={!canEdit}
        />
        <div className="flex gap-2">
          <button className="rounded-md border border-slate-300 p-2 disabled:opacity-50 dark:border-slate-700" disabled={!canDecrease} onClick={() => onUpdate(feature, { currentUses: feature.currentUses - 1 })} type="button" aria-label="Diminuir usos">
            <Minus size={17} />
          </button>
          <button className="rounded-md border border-slate-300 p-2 disabled:opacity-50 dark:border-slate-700" disabled={!canIncrease} onClick={() => onUpdate(feature, { currentUses: feature.currentUses + 1 })} type="button" aria-label="Aumentar usos">
            <Plus size={17} />
          </button>
        </div>
      </div>

      <div className="mt-3 grid gap-3 md:grid-cols-2">
        <SimpleSelect disabled={!canEdit} label="Recuperação" onChange={(value) => onUpdate(feature, { recoveryType: value as RecoveryType })} options={recoveryTypeOptions} value={feature.recoveryType} />
        <textarea
          className="min-h-20 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-sm text-slate-950 shadow-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 disabled:opacity-70 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
          defaultValue={feature.notes}
          disabled={!canEdit}
          onBlur={(event) => onUpdate(feature, { notes: event.target.value })}
          placeholder="Notas"
        />
      </div>
    </div>
  )
}

function compareCharacterFeatures(left: CharacterFeatureResponse, right: CharacterFeatureResponse) {
  return left.name.localeCompare(right.name)
}

function CharacterSpellsSection({
  auth,
  character,
  restVersion,
}: {
  auth: AuthContextValue
  character: CharacterResponse
  restVersion: number
}) {
  const [characterSpells, setCharacterSpells] = useState<CharacterSpellResponse[]>([])
  const [slots, setSlots] = useState<CharacterSpellSlotResponse[]>([])
  const [search, setSearch] = useState('')
  const [libraryResults, setLibraryResults] = useState<SpellResponse[]>([])
  const [message, setMessage] = useState('')
  const [selectedSpell, setSelectedSpell] = useState<CharacterSpellResponse | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)

  useEffect(() => {
    loadCharacterSpells()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [auth.token, character.id, restVersion])

  async function loadCharacterSpells() {
    setIsLoading(true)
    setMessage('')
    try {
      const [spellData, slotData] = await Promise.all([
        apiRequest<CharacterSpellResponse[]>(`/api/characters/${character.id}/spells`, auth.token),
        apiRequest<CharacterSpellSlotResponse[]>(`/api/characters/${character.id}/spell-slots`, auth.token),
      ])
      setCharacterSpells(spellData)
      setSlots(slotData)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao carregar magias do personagem.')
    } finally {
      setIsLoading(false)
    }
  }

  async function searchLibrary(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setMessage('')
    try {
      const query = new URLSearchParams({ name: search, page: '1', pageSize: '8' })
      const result = await apiRequest<PagedResponse<SpellResponse>>(`/api/spells?${query.toString()}`, auth.token)
      setLibraryResults(result.items)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao buscar biblioteca.')
    }
  }

  async function addSpell(spell: SpellResponse) {
    setIsSaving(true)
    setMessage('')
    try {
      const added = await apiRequest<CharacterSpellResponse>(`/api/characters/${character.id}/spells`, auth.token, {
        method: 'POST',
        body: JSON.stringify({
          spellId: spell.id,
          isKnown: true,
          isPrepared: false,
          isFavorite: false,
          notes: '',
        }),
      })
      setCharacterSpells((current) => [...current, added].sort(compareCharacterSpells))
      setLibraryResults((current) => current.filter((item) => item.id !== spell.id))
      setSelectedSpell(added)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao adicionar magia.')
    } finally {
      setIsSaving(false)
    }
  }

  async function updateCharacterSpell(spell: CharacterSpellResponse, patch: Partial<CharacterSpellResponse>) {
    const next = { ...spell, ...patch }
    setCharacterSpells((current) => current.map((item) => (item.id === spell.id ? next : item)))
    if (selectedSpell?.id === spell.id) {
      setSelectedSpell(next)
    }

    try {
      const saved = await apiRequest<CharacterSpellResponse>(
        `/api/characters/${character.id}/spells/${spell.id}`,
        auth.token,
        {
          method: 'PUT',
          body: JSON.stringify({
            isKnown: next.isKnown,
            isPrepared: next.isPrepared,
            isFavorite: next.isFavorite,
            notes: next.notes,
          }),
        },
      )
      setCharacterSpells((current) => current.map((item) => (item.id === saved.id ? saved : item)))
      if (selectedSpell?.id === saved.id) {
        setSelectedSpell(saved)
      }
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao salvar magia.')
      await loadCharacterSpells()
    }
  }

  async function removeSpell(spellId: string) {
    setIsSaving(true)
    setMessage('')
    try {
      await apiRequest<null>(`/api/characters/${character.id}/spells/${spellId}`, auth.token, { method: 'DELETE' })
      setCharacterSpells((current) => current.filter((item) => item.id !== spellId))
      if (selectedSpell?.id === spellId) {
        setSelectedSpell(null)
      }
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao remover magia.')
    } finally {
      setIsSaving(false)
    }
  }

  function patchSlot(level: number, patch: Partial<CharacterSpellSlotResponse>) {
    setSlots((current) =>
      current.map((slot) => (slot.spellLevel === level ? { ...slot, ...patch } : slot)),
    )
  }

  async function saveSlots() {
    setIsSaving(true)
    setMessage('')
    try {
      const saved = await apiRequest<CharacterSpellSlotResponse[]>(
        `/api/characters/${character.id}/spell-slots`,
        auth.token,
        {
          method: 'PUT',
          body: JSON.stringify(slots.map((slot) => ({
            spellLevel: slot.spellLevel,
            totalSlots: slot.totalSlots,
            usedSlots: slot.usedSlots,
          }))),
        },
      )
      setSlots(saved)
      setMessage('Slots salvos.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao salvar slots.')
    } finally {
      setIsSaving(false)
    }
  }

  const cantrips = characterSpells.filter((spell) => spell.level === 0)
  const leveledSpells = Array.from({ length: 9 }, (_, index) => index + 1)
    .map((level) => ({
      level,
      spells: characterSpells.filter((spell) => spell.level === level),
    }))
    .filter((group) => group.spells.length > 0)
  const knownCount = characterSpells.filter((spell) => spell.isKnown).length
  const preparedCount = characterSpells.filter((spell) => spell.isPrepared).length
  const favoriteCount = characterSpells.filter((spell) => spell.isFavorite).length

  if (isLoading) {
    return <PanelText>Carregando magias...</PanelText>
  }

  return (
    <section className="space-y-4">
      {message && <p className="rounded-md bg-emerald-50 p-3 text-sm text-emerald-800 dark:bg-emerald-950 dark:text-emerald-200">{message}</p>}

      <div className="grid gap-4 lg:grid-cols-[minmax(0,1fr)_320px]">
        <div className="space-y-4">
          <div className="grid gap-3 sm:grid-cols-3">
            <StatPill label="Conhecidas" value={knownCount.toString()} />
            <StatPill label="Preparadas" value={preparedCount.toString()} />
            <StatPill label="Favoritas" value={favoriteCount.toString()} />
          </div>

          {character.canEdit && (
            <form className="rounded-lg border border-slate-200 bg-white p-4 dark:border-slate-800 dark:bg-slate-900" onSubmit={searchLibrary}>
              <h3 className="text-lg font-semibold">Adicionar da biblioteca</h3>
              <div className="mt-3 grid gap-3 sm:grid-cols-[1fr_auto]">
                <input
                  className="min-h-11 rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
                  onChange={(event) => setSearch(event.target.value)}
                  placeholder="Buscar por nome"
                  value={search}
                />
                <button className="rounded-md border border-slate-300 px-4 py-2.5 text-sm font-semibold text-slate-700 dark:border-slate-700 dark:text-slate-200" type="submit">
                  Buscar
                </button>
              </div>
              {libraryResults.length > 0 && (
                <div className="mt-3 grid gap-2">
                  {libraryResults.map((spell) => (
                    <div className="flex items-center justify-between gap-3 rounded-md border border-slate-200 p-3 dark:border-slate-800" key={spell.id}>
                      <div>
                        <p className="font-semibold">{spell.name}</p>
                        <p className="text-xs text-slate-500 dark:text-slate-400">{spellLevelLabel(spell.level)} - {spell.school}</p>
                      </div>
                      <button className="rounded-md bg-emerald-600 px-3 py-2 text-xs font-semibold text-white disabled:bg-slate-400" disabled={isSaving} onClick={() => addSpell(spell)} type="button">
                        Adicionar
                      </button>
                    </div>
                  ))}
                </div>
              )}
            </form>
          )}

          {characterSpells.length === 0 ? (
            <PanelText>Nenhuma magia vinculada ainda.</PanelText>
          ) : (
            <div className="space-y-4">
              {cantrips.length > 0 && <CharacterSpellGroup spells={cantrips} title="Truques" onSelect={setSelectedSpell} onUpdate={updateCharacterSpell} onRemove={removeSpell} canEdit={character.canEdit} />}
              {leveledSpells.map((group) => (
                <CharacterSpellGroup
                  canEdit={character.canEdit}
                  key={group.level}
                  onRemove={removeSpell}
                  onSelect={setSelectedSpell}
                  onUpdate={updateCharacterSpell}
                  spells={group.spells}
                  title={`Magias de ${group.level}º nível`}
                />
              ))}
            </div>
          )}
        </div>

        <aside className="space-y-4">
          <div className="rounded-lg border border-slate-200 bg-white p-4 dark:border-slate-800 dark:bg-slate-900">
            <div className="flex items-center justify-between gap-3">
              <h3 className="text-lg font-semibold">Slots</h3>
              {character.canEdit && (
                <button className="rounded-md bg-emerald-600 px-3 py-2 text-xs font-semibold text-white disabled:bg-slate-400" disabled={isSaving} onClick={saveSlots} type="button">
                  Salvar
                </button>
              )}
            </div>
            <div className="mt-3 space-y-2">
              {slots.map((slot) => (
                <div className="grid grid-cols-[48px_1fr_1fr] items-center gap-2 text-sm" key={slot.spellLevel}>
                  <span className="font-semibold">{slot.spellLevel}º</span>
                  <CombatNumber disabled={!character.canEdit} label="Total" min={0} onChange={(value) => patchSlot(slot.spellLevel, { totalSlots: value })} value={slot.totalSlots} />
                  <CombatNumber disabled={!character.canEdit} label="Usados" min={0} onChange={(value) => patchSlot(slot.spellLevel, { usedSlots: value })} value={slot.usedSlots} />
                </div>
              ))}
            </div>
          </div>

          {selectedSpell && (
            <div className="rounded-lg border border-slate-200 bg-white p-4 dark:border-slate-800 dark:bg-slate-900">
              <p className="text-sm font-semibold uppercase text-emerald-700 dark:text-emerald-400">{spellLevelLabel(selectedSpell.level)} - {selectedSpell.school}</p>
              <h3 className="mt-2 text-xl font-semibold">{selectedSpell.name}</h3>
              <div className="mt-3 grid gap-2 text-sm">
                <InfoRow label="Tempo" value={selectedSpell.castingTime} />
                <InfoRow label="Alcance" value={selectedSpell.range} />
                <InfoRow label="Componentes" value={selectedSpell.components} />
                <InfoRow label="Duração" value={selectedSpell.duration} />
                <InfoRow label="Classes" value={selectedSpell.availableClasses} />
              </div>
              <TextBlock label="Descrição" value={selectedSpell.description} />
              <TextBlock label="Em níveis superiores" value={selectedSpell.higherLevelDescription} />
            </div>
          )}
        </aside>
      </div>
    </section>
  )
}

function CharacterSpellGroup({
  canEdit,
  onRemove,
  onSelect,
  onUpdate,
  spells,
  title,
}: {
  canEdit: boolean
  onRemove: (spellId: string) => void
  onSelect: (spell: CharacterSpellResponse) => void
  onUpdate: (spell: CharacterSpellResponse, patch: Partial<CharacterSpellResponse>) => void
  spells: CharacterSpellResponse[]
  title: string
}) {
  return (
    <div className="rounded-lg border border-slate-200 bg-white p-4 dark:border-slate-800 dark:bg-slate-900">
      <h3 className="text-lg font-semibold">{title}</h3>
      <div className="mt-3 grid gap-3">
        {spells.map((spell) => (
          <div className="rounded-md border border-slate-200 p-3 dark:border-slate-800" key={spell.id}>
            <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
              <button className="text-left" onClick={() => onSelect(spell)} type="button">
                <p className="font-semibold">{spell.name}</p>
                <p className="mt-1 text-xs text-slate-500 dark:text-slate-400">
                  {spell.school} - {spell.source || 'Sem fonte'}
                </p>
              </button>
              {canEdit && (
                <button className="self-start rounded-md border border-red-300 p-2 text-red-700 dark:border-red-900 dark:text-red-300" onClick={() => onRemove(spell.id)} type="button" aria-label="Remover magia">
                  <Trash2 size={16} />
                </button>
              )}
            </div>
            <div className="mt-3 grid gap-2 sm:grid-cols-3">
              <label className="flex items-center gap-2 text-sm">
                <input checked={spell.isKnown} disabled={!canEdit} onChange={(event) => onUpdate(spell, { isKnown: event.target.checked })} type="checkbox" />
                Conhecida
              </label>
              <label className="flex items-center gap-2 text-sm">
                <input checked={spell.isPrepared} disabled={!canEdit} onChange={(event) => onUpdate(spell, { isPrepared: event.target.checked })} type="checkbox" />
                Preparada
              </label>
              <label className="flex items-center gap-2 text-sm">
                <input checked={spell.isFavorite} disabled={!canEdit} onChange={(event) => onUpdate(spell, { isFavorite: event.target.checked })} type="checkbox" />
                Favorita
              </label>
            </div>
            <textarea
              className="mt-3 min-h-20 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-sm text-slate-950 shadow-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 disabled:opacity-70 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
              disabled={!canEdit}
              onBlur={(event) => onUpdate(spell, { notes: event.target.value })}
              placeholder="Notas da magia no personagem"
              defaultValue={spell.notes}
            />
          </div>
        ))}
      </div>
    </div>
  )
}

function compareCharacterSpells(left: CharacterSpellResponse, right: CharacterSpellResponse) {
  return left.level - right.level || left.name.localeCompare(right.name)
}

function spellLevelLabel(level: number) {
  return level === 0 ? 'Truque' : `Nível ${level}`
}

function CharacterAssetsSection({
  auth,
  character,
  onCharacterChange,
}: {
  auth: AuthContextValue
  character: CharacterResponse
  onCharacterChange: (character: CharacterResponse) => void
}) {
  const [assets, setAssets] = useState<CharacterAssetResponse[]>([])
  const [message, setMessage] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)

  useEffect(() => {
    apiRequest<CharacterAssetResponse[]>(`/api/characters/${character.id}/assets`, auth.token)
      .then(setAssets)
      .catch((error) => setMessage(error instanceof Error ? error.message : 'Erro ao carregar imagens.'))
      .finally(() => setIsLoading(false))
  }, [auth.token, character.id])

  async function uploadPrimary(file: File | undefined, type: 'avatar' | 'token') {
    if (!file) {
      return
    }

    setIsSaving(true)
    setMessage('')

    const formData = new FormData()
    formData.append('file', file)

    try {
      const updatedCharacter = await apiFormRequest<CharacterResponse>(
        `/api/characters/${character.id}/${type}`,
        auth.token,
        formData,
        'PUT',
      )
      onCharacterChange(updatedCharacter)
      setAssets(await apiRequest<CharacterAssetResponse[]>(`/api/characters/${character.id}/assets`, auth.token))
      setMessage(type === 'avatar' ? 'Avatar atualizado.' : 'Token atualizado.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao enviar imagem.')
    } finally {
      setIsSaving(false)
    }
  }

  async function uploadGallery(file: File | undefined) {
    if (!file) {
      return
    }

    setIsSaving(true)
    setMessage('')

    const formData = new FormData()
    formData.append('file', file)
    formData.append('assetType', 'Gallery')

    try {
      const asset = await apiFormRequest<CharacterAssetResponse>(
        `/api/characters/${character.id}/assets`,
        auth.token,
        formData,
      )
      setAssets((current) => [asset, ...current])
      setMessage('Imagem adicionada à galeria.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao enviar imagem.')
    } finally {
      setIsSaving(false)
    }
  }

  async function deleteAsset(assetId: string) {
    setIsSaving(true)
    setMessage('')

    try {
      await apiRequest<null>(`/api/characters/${character.id}/assets/${assetId}`, auth.token, {
        method: 'DELETE',
      })
      const deletedAsset = assets.find((asset) => asset.id === assetId)
      setAssets((current) => current.filter((asset) => asset.id !== assetId))

      if (deletedAsset?.fileUrl === character.avatarUrl) {
        onCharacterChange({ ...character, avatarUrl: null })
      }
      if (deletedAsset?.fileUrl === character.tokenImageUrl) {
        onCharacterChange({ ...character, tokenImageUrl: null })
      }

      setMessage('Imagem removida.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao remover imagem.')
    } finally {
      setIsSaving(false)
    }
  }

  const galleryAssets = assets.filter((asset) => asset.assetType === 'Gallery')
  const avatarAsset = assets.find((asset) => asset.assetType === 'Avatar' && asset.fileUrl === character.avatarUrl)
    ?? assets.find((asset) => asset.assetType === 'Avatar')
  const tokenAsset = assets.find((asset) => asset.assetType === 'Token' && asset.fileUrl === character.tokenImageUrl)
    ?? assets.find((asset) => asset.assetType === 'Token')

  if (isLoading) {
    return <PanelText>Carregando imagens...</PanelText>
  }

  return (
    <section className="space-y-6">
      <div>
        <h3 className="text-xl font-semibold">Imagens e arquivos</h3>
        <p className="mt-1 text-sm text-slate-600 dark:text-slate-300">
          Upload local de avatar, token e galeria. Formatos: jpg, jpeg, png, webp.
        </p>
      </div>

      {message && (
        <p className="rounded-md bg-emerald-50 p-3 text-sm text-emerald-800 dark:bg-emerald-950 dark:text-emerald-200">
          {message}
        </p>
      )}

      <div className="grid gap-4 md:grid-cols-2">
        <AssetUploadCard
          canEdit={character.canEdit}
          disabled={isSaving}
          imageUrl={character.avatarUrl}
          label="Avatar"
          onRemove={avatarAsset ? () => deleteAsset(avatarAsset.id) : undefined}
          onUpload={(file) => uploadPrimary(file, 'avatar')}
        />
        <AssetUploadCard
          canEdit={character.canEdit}
          disabled={isSaving}
          imageUrl={character.tokenImageUrl}
          label="Token"
          onRemove={tokenAsset ? () => deleteAsset(tokenAsset.id) : undefined}
          onUpload={(file) => uploadPrimary(file, 'token')}
        />
      </div>

      <div className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <h4 className="text-lg font-semibold">Galeria</h4>
          {character.canEdit && (
            <label className="inline-flex cursor-pointer items-center justify-center gap-2 rounded-md bg-emerald-600 px-4 py-3 text-sm font-semibold text-white">
              <Plus size={18} />
              Adicionar imagem
              <input
                accept=".jpg,.jpeg,.png,.webp,image/jpeg,image/png,image/webp"
                className="sr-only"
                disabled={isSaving}
                onChange={(event) => {
                  void uploadGallery(event.target.files?.[0])
                  event.target.value = ''
                }}
                type="file"
              />
            </label>
          )}
        </div>

        <div className="mt-5 grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
          {galleryAssets.length === 0 ? (
            <p className="text-sm text-slate-500 dark:text-slate-400">Galeria vazia.</p>
          ) : galleryAssets.map((asset) => (
            <div className="overflow-hidden rounded-lg border border-slate-200 dark:border-slate-800" key={asset.id}>
              <img alt="" className="aspect-video w-full object-cover" src={assetUrl(asset.fileUrl)} />
              <div className="flex items-center justify-between gap-3 p-3">
                <p className="truncate text-sm text-slate-500 dark:text-slate-400">{asset.fileName}</p>
                {character.canEdit && (
                  <button
                    aria-label="Remover imagem"
                    className="rounded-md border border-red-300 p-2 text-red-700 dark:border-red-900 dark:text-red-300"
                    disabled={isSaving}
                    onClick={() => deleteAsset(asset.id)}
                    type="button"
                  >
                    <Trash2 size={17} />
                  </button>
                )}
              </div>
            </div>
          ))}
        </div>
      </div>
    </section>
  )
}

function AssetUploadCard({
  canEdit,
  disabled,
  imageUrl,
  label,
  onRemove,
  onUpload,
}: {
  canEdit: boolean
  disabled: boolean
  imageUrl?: string | null
  label: string
  onRemove?: () => void
  onUpload: (file: File | undefined) => void
}) {
  return (
    <div className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
      <h4 className="text-lg font-semibold">{label}</h4>
      <div className="mt-4 overflow-hidden rounded-lg border border-slate-200 bg-slate-100 dark:border-slate-800 dark:bg-slate-950">
        {imageUrl ? (
          <img alt="" className="aspect-video w-full object-cover" src={assetUrl(imageUrl)} />
        ) : (
          <div className="grid aspect-video place-items-center text-sm text-slate-500 dark:text-slate-400">
            Sem imagem
          </div>
        )}
      </div>
      {canEdit && (
        <div className="mt-4 flex flex-col gap-2 sm:flex-row">
          <label className="inline-flex cursor-pointer items-center justify-center gap-2 rounded-md bg-emerald-600 px-4 py-3 text-sm font-semibold text-white">
            <Plus size={18} />
            Enviar {label.toLowerCase()}
            <input
              accept=".jpg,.jpeg,.png,.webp,image/jpeg,image/png,image/webp"
              className="sr-only"
              disabled={disabled}
              onChange={(event) => {
                onUpload(event.target.files?.[0])
                event.target.value = ''
              }}
              type="file"
            />
          </label>
          {onRemove && (
            <button
              className="inline-flex items-center justify-center gap-2 rounded-md border border-red-300 px-4 py-3 text-sm font-semibold text-red-700 disabled:opacity-70 dark:border-red-900 dark:text-red-300"
              disabled={disabled}
              onClick={onRemove}
              type="button"
            >
              <Trash2 size={18} />
              Remover
            </button>
          )}
        </div>
      )}
    </div>
  )
}

function assetUrl(fileUrl: string) {
  return fileUrl.startsWith('http') ? fileUrl : `${API_ORIGIN}${fileUrl}`
}

const itemTypeOptions: Array<{ label: string; value: ItemType }> = [
  { label: 'Arma', value: 'Weapon' },
  { label: 'Armadura', value: 'Armor' },
  { label: 'Consumível', value: 'Consumable' },
  { label: 'Ferramenta', value: 'Tool' },
  { label: 'Item mágico', value: 'MagicItem' },
  { label: 'Tesouro', value: 'Treasure' },
  { label: 'Outro', value: 'Other' },
]

function CharacterInventorySection({
  auth,
  character,
}: {
  auth: AuthContextValue
  character: CharacterResponse
}) {
  const [items, setItems] = useState<CharacterInventoryItemResponse[]>([])
  const [currency, setCurrency] = useState<CharacterCurrencyResponse | null>(null)
  const [draft, setDraft] = useState<CharacterInventoryItemResponse>(emptyInventoryItem(character.id))
  const [editingItemId, setEditingItemId] = useState<string | null>(null)
  const [message, setMessage] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)

  useEffect(() => {
    Promise.all([
      apiRequest<CharacterInventoryItemResponse[]>(`/api/characters/${character.id}/inventory`, auth.token),
      apiRequest<CharacterCurrencyResponse>(`/api/characters/${character.id}/currency`, auth.token),
    ])
      .then(([itemData, currencyData]) => {
        setItems(itemData)
        setCurrency(currencyData)
      })
      .catch((error) => setMessage(error instanceof Error ? error.message : 'Erro ao carregar inventário.'))
      .finally(() => setIsLoading(false))
  }, [auth.token, character.id])

  const totalWeight = useMemo(
    () => items.reduce((sum, item) => sum + item.quantity * item.weight, 0),
    [items],
  )

  function patchDraft(patch: Partial<CharacterInventoryItemResponse>) {
    setDraft((current) => ({ ...current, ...patch }))
  }

  function patchCurrency(patch: Partial<CharacterCurrencyResponse>) {
    setCurrency((current) => (current ? { ...current, ...patch } : current))
  }

  function editItem(item: CharacterInventoryItemResponse) {
    setEditingItemId(item.id)
    setDraft(item)
    setMessage('')
  }

  function newItem() {
    setEditingItemId(null)
    setDraft(emptyInventoryItem(character.id))
    setMessage('')
  }

  async function saveCurrency() {
    if (!currency) {
      return
    }

    setIsSaving(true)
    setMessage('')

    try {
      setCurrency(await apiRequest<CharacterCurrencyResponse>(`/api/characters/${character.id}/currency`, auth.token, {
        method: 'PUT',
        body: JSON.stringify({
          copper: currency.copper,
          silver: currency.silver,
          electrum: currency.electrum,
          gold: currency.gold,
          platinum: currency.platinum,
        }),
      }))
      setMessage('Moedas salvas.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao salvar moedas.')
    } finally {
      setIsSaving(false)
    }
  }

  async function saveItem(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setIsSaving(true)
    setMessage('')

    const payload = {
      name: draft.name,
      description: draft.description,
      quantity: draft.quantity,
      weight: draft.weight,
      value: draft.value,
      itemType: draft.itemType,
      equipped: draft.equipped,
      attuned: draft.attuned,
      notes: draft.notes,
    }

    try {
      const saved = await apiRequest<CharacterInventoryItemResponse>(
        editingItemId
          ? `/api/characters/${character.id}/inventory/${editingItemId}`
          : `/api/characters/${character.id}/inventory`,
        auth.token,
        {
          method: editingItemId ? 'PUT' : 'POST',
          body: JSON.stringify(payload),
        },
      )

      setItems((current) =>
        editingItemId
          ? current.map((item) => (item.id === saved.id ? saved : item))
          : [...current, saved].sort((left, right) => left.name.localeCompare(right.name)),
      )
      setDraft(emptyInventoryItem(character.id))
      setEditingItemId(null)
      setMessage('Item salvo.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao salvar item.')
    } finally {
      setIsSaving(false)
    }
  }

  async function deleteItem(itemId: string) {
    setIsSaving(true)
    setMessage('')

    try {
      await apiRequest<null>(`/api/characters/${character.id}/inventory/${itemId}`, auth.token, {
        method: 'DELETE',
      })
      setItems((current) => current.filter((item) => item.id !== itemId))
      if (editingItemId === itemId) {
        newItem()
      }
      setMessage('Item excluído.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao excluir item.')
    } finally {
      setIsSaving(false)
    }
  }

  if (isLoading || !currency) {
    return <PanelText>Carregando inventário...</PanelText>
  }

  return (
    <section className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h3 className="text-xl font-semibold">Inventário</h3>
          <p className="mt-1 text-sm text-slate-600 dark:text-slate-300">
            Itens manuais, moedas e peso total simples.
          </p>
        </div>
        <div className="rounded-md bg-slate-100 px-4 py-3 text-sm font-semibold dark:bg-slate-900">
          Peso total: {formatDecimal(totalWeight)}
        </div>
      </div>

      {message && (
        <p className="rounded-md bg-emerald-50 p-3 text-sm text-emerald-800 dark:bg-emerald-950 dark:text-emerald-200">
          {message}
        </p>
      )}

      <div className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <h4 className="text-lg font-semibold">Moedas</h4>
          {character.canEdit && (
            <button className="rounded-md bg-emerald-600 px-4 py-2 text-sm font-semibold text-white disabled:bg-slate-400" disabled={isSaving} onClick={saveCurrency} type="button">
              Salvar moedas
            </button>
          )}
        </div>
        <div className="mt-4 grid gap-3 sm:grid-cols-2 lg:grid-cols-5">
          <CombatNumber label="Cobre" disabled={!character.canEdit} min={0} onChange={(value) => patchCurrency({ copper: value })} value={currency.copper} />
          <CombatNumber label="Prata" disabled={!character.canEdit} min={0} onChange={(value) => patchCurrency({ silver: value })} value={currency.silver} />
          <CombatNumber label="Electro" disabled={!character.canEdit} min={0} onChange={(value) => patchCurrency({ electrum: value })} value={currency.electrum} />
          <CombatNumber label="Ouro" disabled={!character.canEdit} min={0} onChange={(value) => patchCurrency({ gold: value })} value={currency.gold} />
          <CombatNumber label="Platina" disabled={!character.canEdit} min={0} onChange={(value) => patchCurrency({ platinum: value })} value={currency.platinum} />
        </div>
      </div>

      <div className="grid gap-6 xl:grid-cols-[1fr_1.1fr]">
        <div className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
          <div className="flex items-center justify-between gap-3">
            <h4 className="text-lg font-semibold">Itens</h4>
            {character.canEdit && (
              <button className="text-sm font-medium text-emerald-700 dark:text-emerald-300" onClick={newItem} type="button">
                Novo item
              </button>
            )}
          </div>
          <div className="mt-4 grid gap-3">
            {items.length === 0 ? (
              <p className="text-sm text-slate-500 dark:text-slate-400">Inventário vazio.</p>
            ) : items.map((item) => (
              <div className="rounded-md border border-slate-200 p-4 dark:border-slate-800" key={item.id}>
                <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
                  <div>
                    <div className="flex flex-wrap items-center gap-2">
                      <h5 className="font-semibold">{item.name}</h5>
                      <span className="rounded-full bg-slate-100 px-2 py-0.5 text-xs dark:bg-slate-800">{item.itemTypeLabel}</span>
                      {item.equipped && <span className="rounded-full bg-emerald-100 px-2 py-0.5 text-xs text-emerald-800 dark:bg-emerald-900 dark:text-emerald-100">Equipado</span>}
                      {item.attuned && <span className="rounded-full bg-violet-100 px-2 py-0.5 text-xs text-violet-800 dark:bg-violet-900 dark:text-violet-100">Sintonizado</span>}
                    </div>
                    <p className="mt-1 text-sm text-slate-600 dark:text-slate-300">
                      Qtd {item.quantity} · peso {formatDecimal(item.weight)} · total {formatDecimal(item.totalWeight)} · valor {formatDecimal(item.value)}
                    </p>
                    {item.description && <p className="mt-2 whitespace-pre-wrap text-sm text-slate-500 dark:text-slate-400">{item.description}</p>}
                    {item.notes && <p className="mt-2 whitespace-pre-wrap text-sm text-slate-500 dark:text-slate-400">{item.notes}</p>}
                  </div>
                  {character.canEdit && (
                    <div className="flex gap-2">
                      <button className="rounded-md border border-slate-300 p-2 dark:border-slate-700" onClick={() => editItem(item)} type="button" aria-label="Editar item">
                        <Edit size={17} />
                      </button>
                      <button className="rounded-md border border-red-300 p-2 text-red-700 dark:border-red-900 dark:text-red-300" onClick={() => deleteItem(item.id)} type="button" aria-label="Excluir item">
                        <Trash2 size={17} />
                      </button>
                    </div>
                  )}
                </div>
              </div>
            ))}
          </div>
        </div>

        {character.canEdit && (
          <form className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900" onSubmit={saveItem}>
            <div className="flex items-center justify-between gap-3">
              <h4 className="text-lg font-semibold">{editingItemId ? 'Editar item' : 'Novo item'}</h4>
              {editingItemId && (
                <button className="text-sm font-medium text-slate-500 dark:text-slate-400" onClick={newItem} type="button">
                  Cancelar
                </button>
              )}
            </div>
            <div className="mt-4 grid gap-3">
              <CombatText label="Nome" onChange={(value) => patchDraft({ name: value })} value={draft.name} />
              <label className="block">
                <span className="text-sm font-medium text-slate-700 dark:text-slate-200">Tipo</span>
                <select
                  className="mt-1 min-h-11 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
                  onChange={(event) => patchDraft({ itemType: event.target.value as ItemType })}
                  value={draft.itemType}
                >
                  {itemTypeOptions.map((option) => (
                    <option key={option.value} value={option.value}>{option.label}</option>
                  ))}
                </select>
              </label>
              <div className="grid gap-3 sm:grid-cols-3">
                <CombatNumber label="Quantidade" min={0} onChange={(value) => patchDraft({ quantity: value })} value={draft.quantity} />
                <CombatNumber label="Peso" min={0} onChange={(value) => patchDraft({ weight: value })} value={draft.weight} />
                <CombatNumber label="Valor" min={0} onChange={(value) => patchDraft({ value })} value={draft.value} />
              </div>
              <CombatTextArea label="Descrição" onChange={(value) => patchDraft({ description: value })} value={draft.description} />
              <CombatTextArea label="Notas" onChange={(value) => patchDraft({ notes: value })} value={draft.notes} />
              <div className="grid gap-3 sm:grid-cols-2">
                <label className="flex items-center gap-2 text-sm">
                  <input checked={draft.equipped} className="size-4" onChange={(event) => patchDraft({ equipped: event.target.checked })} type="checkbox" />
                  Equipado
                </label>
                <label className="flex items-center gap-2 text-sm">
                  <input checked={draft.attuned} className="size-4" onChange={(event) => patchDraft({ attuned: event.target.checked })} type="checkbox" />
                  Sintonizado
                </label>
              </div>
              <button className="inline-flex items-center justify-center gap-2 rounded-md bg-emerald-600 px-4 py-3 text-sm font-semibold text-white disabled:bg-slate-400" disabled={isSaving} type="submit">
                <Plus size={18} />
                {isSaving ? 'Salvando...' : 'Salvar item'}
              </button>
            </div>
          </form>
        )}
      </div>
    </section>
  )
}

function emptyInventoryItem(characterId: string): CharacterInventoryItemResponse {
  return {
    id: '',
    characterId,
    name: '',
    description: '',
    quantity: 1,
    weight: 0,
    value: 0,
    itemType: 'Other',
    itemTypeLabel: 'Outro',
    equipped: false,
    attuned: false,
    notes: '',
    totalWeight: 0,
    canEdit: true,
  }
}

function emptyAttack(): CharacterAttackResponse {
  return {
    id: '',
    name: '',
    attackBonus: 0,
    damage: '',
    damageType: '',
    range: '',
    usesAttribute: null,
    usesAttributeLabel: null,
    notes: '',
  }
}

function CombatNumber({
  disabled,
  label,
  min,
  onChange,
  value,
}: {
  disabled?: boolean
  label: string
  min?: number
  onChange: (value: number) => void
  value: number
}) {
  return (
    <label className="block">
      <span className="text-sm font-medium text-slate-700 dark:text-slate-200">{label}</span>
      <input
        className="mt-1 min-h-11 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition placeholder:text-slate-400 focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 disabled:opacity-70 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
        disabled={disabled}
        min={min}
        onChange={(event) => onChange(Number(event.target.value))}
        step="any"
        type="number"
        value={value}
      />
    </label>
  )
}

function CombatText({
  disabled,
  label,
  onChange,
  value,
}: {
  disabled?: boolean
  label: string
  onChange: (value: string) => void
  value: string
}) {
  return (
    <label className="block">
      <span className="text-sm font-medium text-slate-700 dark:text-slate-200">{label}</span>
      <input
        className="mt-1 min-h-11 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition placeholder:text-slate-400 focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 disabled:opacity-70 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
        disabled={disabled}
        onChange={(event) => onChange(event.target.value)}
        value={value}
      />
    </label>
  )
}

function CombatTextArea({
  label,
  onChange,
  value,
}: {
  label: string
  onChange: (value: string) => void
  value: string
}) {
  return (
    <label className="block">
      <span className="text-sm font-medium text-slate-700 dark:text-slate-200">{label}</span>
      <textarea
        className="mt-1 min-h-24 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition placeholder:text-slate-400 focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
        onChange={(event) => onChange(event.target.value)}
        value={value}
      />
    </label>
  )
}

function NumberField({
  disabled,
  label,
  min,
  onChange,
  value,
}: {
  disabled?: boolean
  label: string
  min?: number
  onChange: (value: number) => void
  value: number
}) {
  return (
    <label className="block">
      <span className="text-sm font-medium text-slate-700 dark:text-slate-200">{label}</span>
      <input
        className="mt-1 min-h-11 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition placeholder:text-slate-400 focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 disabled:opacity-70 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
        disabled={disabled}
        min={min}
        onChange={(event) => onChange(Number(event.target.value))}
        type="number"
        value={value}
      />
    </label>
  )
}

function TabButton({
  active,
  children,
  onClick,
}: {
  active: boolean
  children: ReactNode
  onClick: () => void
}) {
  return (
    <button
      className={`min-h-11 whitespace-nowrap border-b-2 px-3 py-3 text-sm font-semibold transition ${
        active
          ? 'border-emerald-600 text-emerald-700 dark:text-emerald-300'
          : 'border-transparent text-slate-500 hover:text-slate-800 dark:text-slate-400 dark:hover:text-slate-200'
      }`}
      onClick={onClick}
      type="button"
    >
      {children}
    </button>
  )
}

function openCharacterPdf(pdfWindow: Window, data: CharacterPdfData) {
  const { character, playerName, abilities, skills, combat, attacks, spells, features, inventory } = data
  const mainSkills = (skills.some((skill) => skill.isProficient || skill.isExpertise || skill.customBonus !== 0)
    ? skills.filter((skill) => skill.isProficient || skill.isExpertise || skill.customBonus !== 0)
    : skills.slice().sort((left, right) => right.finalValue - left.finalValue).slice(0, 8)
  ).sort((left, right) => left.label.localeCompare(right.label))
  const spellsByLevel = Array.from(new Set(spells.map((spell) => spell.level))).sort((left, right) => left - right)
  const title = `Ficha - ${character.name || 'personagem'}`
  const availableHitDice = (combat?.availableHitDice ?? character.availableHitDice) || '-'
  const totalHitDice = (combat?.totalHitDice ?? character.totalHitDice) || '-'

  const documentHtml = `<!doctype html>
<html lang="pt-BR">
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>${escapeHtml(title)}</title>
  <style>
    @page { size: A4; margin: 12mm; }
    * { box-sizing: border-box; }
    body {
      margin: 0;
      color: #0f172a;
      background: #f8fafc;
      font-family: Inter, ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
      font-size: 12px;
      line-height: 1.45;
    }
    main {
      max-width: 960px;
      margin: 0 auto;
      padding: 18px;
      background: white;
    }
    header {
      display: grid;
      gap: 10px;
      padding-bottom: 14px;
      border-bottom: 2px solid #0f172a;
    }
    h1, h2, h3, p { margin: 0; }
    h1 { font-size: 28px; line-height: 1.1; }
    h2 { font-size: 15px; margin-bottom: 8px; }
    h3 { font-size: 13px; margin-bottom: 6px; color: #334155; }
    .muted { color: #64748b; }
    .grid { display: grid; gap: 10px; }
    .meta { grid-template-columns: repeat(4, minmax(0, 1fr)); }
    .cards { grid-template-columns: repeat(4, minmax(0, 1fr)); }
    .two { grid-template-columns: repeat(2, minmax(0, 1fr)); }
    .card {
      break-inside: avoid;
      border: 1px solid #cbd5e1;
      border-radius: 8px;
      padding: 10px;
      background: #ffffff;
    }
    .label {
      display: block;
      margin-bottom: 2px;
      color: #64748b;
      font-size: 10px;
      font-weight: 700;
      letter-spacing: .04em;
      text-transform: uppercase;
    }
    .value { font-weight: 700; }
    .section { margin-top: 12px; break-inside: avoid; }
    .list { display: grid; gap: 6px; }
    .row {
      display: grid;
      gap: 6px;
      grid-template-columns: minmax(0, 1.2fr) minmax(0, 2fr);
      padding: 6px 0;
      border-top: 1px solid #e2e8f0;
    }
    .row:first-child { border-top: 0; }
    .badges { display: flex; flex-wrap: wrap; gap: 4px; margin-top: 3px; }
    .badge {
      border: 1px solid #cbd5e1;
      border-radius: 999px;
      padding: 1px 6px;
      color: #334155;
      font-size: 10px;
      font-weight: 700;
    }
    .notes {
      white-space: pre-wrap;
      overflow-wrap: anywhere;
    }
    footer {
      margin-top: 16px;
      padding-top: 8px;
      border-top: 1px solid #cbd5e1;
      color: #64748b;
      font-size: 10px;
    }
    @media print {
      body { background: white; }
      main { padding: 0; max-width: none; }
      .no-print { display: none; }
    }
    @media (max-width: 720px) {
      main { padding: 14px; }
      .meta, .cards, .two { grid-template-columns: repeat(2, minmax(0, 1fr)); }
      .row { grid-template-columns: 1fr; }
    }
  </style>
</head>
<body>
  <main>
    <button class="no-print" onclick="window.print()" style="margin-bottom: 12px; border: 1px solid #cbd5e1; border-radius: 8px; background: #059669; color: white; padding: 10px 14px; font-weight: 700;">Salvar como PDF</button>
    <header>
      <div>
        <p class="muted">Ficha resumida de personagem</p>
        <h1>${escapeHtml(character.name || 'Sem nome')}</h1>
        <p class="muted">${escapeHtml(character.nickname || character.campaignName || 'Dados exportados da ficha cadastrada')}</p>
      </div>
      <div class="grid meta">
        ${pdfFact('Jogador', playerName)}
        ${pdfFact('Campanha', character.campaignName ?? 'Sem campanha')}
        ${pdfFact('Classe', [character.mainClass, character.subclass].filter(Boolean).join(' / ') || '-')}
        ${pdfFact('Nível', character.totalLevel.toString())}
        ${pdfFact('Espécie/Raça', character.species || '-')}
        ${pdfFact('Antecedente', character.background || '-')}
        ${pdfFact('Alinhamento', character.alignment || '-')}
        ${pdfFact('Bônus prof.', formatSigned(character.proficiencyBonus))}
      </div>
    </header>

    <section class="section grid cards">
      ${pdfFact('Vida', `${combat?.currentHitPoints ?? character.currentHitPoints}/${combat?.maxHitPoints ?? character.maxHitPoints}`)}
      ${pdfFact('Vida temp.', `${combat?.temporaryHitPoints ?? character.temporaryHitPoints}`)}
      ${pdfFact('CA', `${combat?.armorClass ?? character.armorClass}`)}
      ${pdfFact('Iniciativa', formatSigned(combat?.initiative ?? character.initiative))}
      ${pdfFact('Deslocamento', `${combat?.speed ?? character.speed}`)}
      ${pdfFact('Dados de vida', `${availableHitDice} / ${totalHitDice}`)}
      ${pdfFact('Experiência', character.experience.toString())}
      ${pdfFact('Inspiração', character.inspiration ? 'Sim' : 'Não')}
    </section>

    <section class="section grid two">
      ${pdfSection('Atributos', abilities.length === 0 ? pdfEmpty('Nenhum atributo carregado.') : `<div class="grid cards">${abilities.map((ability) => pdfFact(ability.label, `${ability.score} (${formatSigned(ability.modifier)})`)).join('')}</div>`)}
      ${pdfSection('Perícias principais', mainSkills.length === 0 ? pdfEmpty('Nenhuma perícia carregada.') : `<div class="list">${mainSkills.map((skill) => `<div class="row"><strong>${escapeHtml(skill.label)}</strong><span>${formatSigned(skill.finalValue)}${skill.isExpertise ? ' - expertise' : skill.isProficient ? ' - proficiente' : ''}</span></div>`).join('')}</div>`)}
    </section>

    ${pdfSection('Ataques', attacks.length === 0 ? pdfEmpty('Nenhum ataque cadastrado.') : `<div class="list">${attacks.map((attack) => `<div class="row"><div><strong>${escapeHtml(attack.name || 'Ataque')}</strong><div class="muted">${escapeHtml(attack.range || '-')}</div></div><div>${escapeHtml(formatSigned(attack.attackBonus))} para acertar; ${escapeHtml(attack.damage || '-')} ${escapeHtml(attack.damageType || '')}${attack.notes ? `<div class="notes muted">${escapeHtml(attack.notes)}</div>` : ''}</div></div>`).join('')}</div>`)}

    ${pdfSection('Magias vinculadas', spells.length === 0 ? pdfEmpty('Nenhuma magia vinculada.') : spellsByLevel.map((level) => `<h3>${escapeHtml(spellLevelLabel(level))}</h3><div class="list">${spells.filter((spell) => spell.level === level).map((spell) => `<div class="row"><div><strong>${escapeHtml(spell.name)}</strong><div class="muted">${escapeHtml(spell.school || '-')}</div></div><div><div class="badges">${spell.isKnown ? '<span class="badge">Conhecida</span>' : ''}${spell.isPrepared ? '<span class="badge">Preparada</span>' : ''}${spell.isFavorite ? '<span class="badge">Favorita</span>' : ''}${spell.isRitual ? '<span class="badge">Ritual</span>' : ''}${spell.isConcentration ? '<span class="badge">Concentração</span>' : ''}</div>${spell.notes ? `<div class="notes muted">${escapeHtml(spell.notes)}</div>` : ''}</div></div>`).join('')}</div>`).join(''))}

    ${pdfSection('Talentos e características', features.length === 0 ? pdfEmpty('Nenhum talento/característica cadastrado.') : `<div class="list">${features.map((feature) => `<div class="row"><div><strong>${escapeHtml(feature.name || feature.customName || 'Característica')}</strong><div class="muted">${escapeHtml(feature.type ? featureTypeLabel(feature.type) : feature.typeLabel || 'Manual')}</div></div><div>${feature.maxUses > 0 ? `Usos: ${feature.currentUses}/${feature.maxUses}. ` : ''}${escapeHtml(feature.recoveryTypeLabel || recoveryTypeLabel(feature.recoveryType))}${feature.notes ? `<div class="notes muted">${escapeHtml(feature.notes)}</div>` : ''}</div></div>`).join('')}</div>`)}

    ${pdfSection('Inventário resumido', inventory.length === 0 ? pdfEmpty('Nenhum item cadastrado.') : `<div class="list">${inventory.map((item) => `<div class="row"><div><strong>${escapeHtml(item.name)}</strong><div class="muted">${escapeHtml(item.itemTypeLabel || item.itemType)}</div></div><div>Qtd. ${item.quantity}; peso ${formatDecimal(item.totalWeight || item.quantity * item.weight)}; valor ${formatDecimal(item.value)}${item.equipped ? ' - equipado' : ''}${item.attuned ? ' - sintonizado' : ''}${item.notes ? `<div class="notes muted">${escapeHtml(item.notes)}</div>` : ''}</div></div>`).join('')}</div>`)}

    ${pdfSection('Anotações rápidas', character.quickNotes ? `<p class="notes">${escapeHtml(character.quickNotes)}</p>` : pdfEmpty('Sem anotações rápidas.'))}

    <footer>
      Exportado em ${escapeHtml(new Date().toLocaleString('pt-BR'))}. O PDF contém apenas dados cadastrados na ficha e nas bibliotecas locais acessíveis ao usuário.
    </footer>
  </main>
  <script>
    window.addEventListener('load', () => {
      window.focus();
      setTimeout(() => window.print(), 250);
    });
  </script>
</body>
</html>`

  pdfWindow.document.open()
  pdfWindow.document.write(documentHtml)
  pdfWindow.document.close()
}

function pdfSection(title: string, content: string) {
  return `<section class="section card"><h2>${escapeHtml(title)}</h2>${content}</section>`
}

function pdfFact(label: string, value: string) {
  return `<div class="card"><span class="label">${escapeHtml(label)}</span><span class="value">${escapeHtml(value || '-')}</span></div>`
}

function pdfEmpty(message: string) {
  return `<p class="muted">${escapeHtml(message)}</p>`
}

function escapeHtml(value: unknown) {
  return String(value ?? '')
    .replaceAll('&', '&amp;')
    .replaceAll('<', '&lt;')
    .replaceAll('>', '&gt;')
    .replaceAll('"', '&quot;')
    .replaceAll("'", '&#039;')
}

function recoveryTypeLabel(type: RecoveryType) {
  if (type === 'ShortRest') {
    return 'Descanso curto'
  }
  if (type === 'LongRest') {
    return 'Descanso longo'
  }
  return 'Manual'
}

function calculateModifier(score: number) {
  return Math.floor((score - 10) / 2)
}

function formatSigned(value: number) {
  return value >= 0 ? `+${value}` : value.toString()
}

function formatDecimal(value: number) {
  return new Intl.NumberFormat('pt-BR', { maximumFractionDigits: 2 }).format(value)
}

function lowerFirst(value: string) {
  return `${value.slice(0, 1).toLowerCase()}${value.slice(1)}`
}

function AvatarImage({ name, src }: { name: string; src?: string | null }) {
  if (src) {
    return (
      <img
        alt=""
        className="size-20 rounded-lg border border-slate-200 object-cover dark:border-slate-800"
        src={src}
      />
    )
  }

  return (
    <span className="flex size-20 shrink-0 items-center justify-center rounded-lg bg-emerald-50 text-2xl font-semibold text-emerald-700 dark:bg-emerald-950 dark:text-emerald-300">
      {name.trim().slice(0, 1).toUpperCase() || '?'}
    </span>
  )
}

function StatPill({ label, value }: { label: string; value: string }) {
  return (
    <span className="rounded-md bg-slate-100 px-2 py-1 text-slate-700 dark:bg-slate-950 dark:text-slate-200">
      {label}: {value}
    </span>
  )
}

const spellSchools = [
  'Abjuração',
  'Adivinhação',
  'Conjuração',
  'Encantamento',
  'Evocação',
  'Ilusão',
  'Necromancia',
  'Transmutação',
]

const spellExamples: SpellPayload[] = [
  {
    name: 'Brilho de Vigília',
    englishName: 'Watchlight',
    level: 0,
    school: 'Evocação',
    castingTime: '1 ação',
    range: '18 metros',
    components: 'V, S',
    material: '',
    duration: '1 minuto',
    isConcentration: false,
    isRitual: false,
    description: 'Um ponto de luz suave marca um objeto à vista. A luz não causa dano e pode ter cor escolhida.',
    higherLevelDescription: '',
    availableClasses: 'Bardo, Mago',
    source: 'Exemplo fictício local',
    isHomebrew: true,
    visibility: 'Private',
    campaignId: null,
  },
  {
    name: 'Passo de Névoa Mansa',
    englishName: 'Gentle Mist Step',
    level: 1,
    school: 'Conjuração',
    castingTime: '1 ação bônus',
    range: 'Pessoal',
    components: 'V',
    material: '',
    duration: 'Instantânea',
    isConcentration: false,
    isRitual: false,
    description: 'Névoa breve cobre seus pés e permite mover-se alguns metros para espaço desocupado visível.',
    higherLevelDescription: 'Com níveis maiores, aumente o alcance conforme regra da mesa.',
    availableClasses: 'Feiticeiro, Mago',
    source: 'Exemplo fictício local',
    isHomebrew: true,
    visibility: 'Private',
    campaignId: null,
  },
]

const emptySpellPayload: SpellPayload = {
  name: '',
  englishName: '',
  level: 0,
  school: 'Evocação',
  castingTime: '',
  range: '',
  components: '',
  material: '',
  duration: '',
  isConcentration: false,
  isRitual: false,
  description: '',
  higherLevelDescription: '',
  availableClasses: '',
  source: 'Homebrew local',
  isHomebrew: true,
  visibility: 'Private',
  campaignId: null,
}

function SpellsPage({ auth }: { auth: AuthContextValue }) {
  const [spells, setSpells] = useState<PagedResponse<SpellResponse> | null>(null)
  const [campaigns, setCampaigns] = useState<CampaignSummaryResponse[]>([])
  const [selectedSpell, setSelectedSpell] = useState<SpellResponse | null>(null)
  const [editingSpellId, setEditingSpellId] = useState<string | null>(null)
  const [draft, setDraft] = useState<SpellPayload>(emptySpellPayload)
  const [message, setMessage] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)
  const [isImporting, setIsImporting] = useState(false)
  const [isSpellFormOpen, setIsSpellFormOpen] = useState(false)
  const [filters, setFilters] = useState({
    name: '',
    level: '',
    school: '',
    className: '',
    isConcentration: '',
    isRitual: '',
    source: '',
    isHomebrew: '',
    visibility: '',
    page: 1,
    pageSize: 12,
  })

  const masterCampaigns = campaigns.filter((campaign) => campaign.currentUserRole === 'Master')
  const visibilityOptions = contentVisibilityOptions(masterCampaigns.length > 0)

  useEffect(() => {
    apiRequest<CampaignSummaryResponse[]>('/api/campaigns', auth.token)
      .then(setCampaigns)
      .catch(() => setCampaigns([]))
  }, [auth.token])

  useEffect(() => {
    loadSpells()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [auth.token, filters.page])

  async function loadSpells(nextFilters = filters) {
    setIsLoading(true)
    setMessage('')

    const query = new URLSearchParams()
    Object.entries(nextFilters).forEach(([key, value]) => {
      if (value === '' || value === null || value === undefined) {
        return
      }
      query.set(key === 'className' ? 'class' : key, String(value))
    })

    try {
      setSpells(await apiRequest<PagedResponse<SpellResponse>>(`/api/spells?${query.toString()}`, auth.token))
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao carregar magias.')
    } finally {
      setIsLoading(false)
    }
  }

  function updateFilter(key: keyof typeof filters, value: string | number) {
    setFilters((current) => ({ ...current, [key]: value, page: key === 'page' ? Number(value) : 1 }))
  }

  function startNewSpell(example?: SpellPayload) {
    setEditingSpellId(null)
    setSelectedSpell(null)
    const next = { ...(example ?? emptySpellPayload) }
    setDraft(masterCampaigns.length > 0 ? next : { ...next, visibility: next.visibility === 'Campaign' ? 'Private' : next.visibility, campaignId: null })
    setMessage('')
    setIsSpellFormOpen(true)
  }

  function startEditSpell(spell: SpellResponse) {
    setEditingSpellId(spell.id)
    setSelectedSpell(spell)
    setDraft(toSpellPayload(spell))
    setMessage('')
    setIsSpellFormOpen(true)
  }

  function patchDraft(patch: Partial<SpellPayload>) {
    setDraft((current) => {
      const next = { ...current, ...patch }
      if (next.visibility !== 'Campaign') {
        next.campaignId = null
      }
      return next
    })
  }

  async function selectSpell(id: string) {
    setMessage('')
    try {
      const spell = await apiRequest<SpellResponse>(`/api/spells/${id}`, auth.token)
      setSelectedSpell(spell)
      setEditingSpellId(null)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao abrir magia.')
    }
  }

  async function saveSpell(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setIsSaving(true)
    setMessage('')

    try {
      const spell = await apiRequest<SpellResponse>(
        editingSpellId ? `/api/spells/${editingSpellId}` : '/api/spells',
        auth.token,
        {
          method: editingSpellId ? 'PUT' : 'POST',
          body: JSON.stringify(normalizeSpellPayload(draft)),
        },
      )
      setSelectedSpell(spell)
      setEditingSpellId(null)
      setDraft(emptySpellPayload)
      setIsSpellFormOpen(false)
      await loadSpells()
      setMessage('Magia salva.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao salvar magia.')
    } finally {
      setIsSaving(false)
    }
  }

  async function deleteSpell(spellId: string) {
    setIsSaving(true)
    setMessage('')

    try {
      await apiRequest<null>(`/api/spells/${spellId}`, auth.token, { method: 'DELETE' })
      setSelectedSpell(null)
      setEditingSpellId(null)
      await loadSpells()
      setMessage('Magia excluída.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao excluir magia.')
    } finally {
      setIsSaving(false)
    }
  }

  async function importOpen5e() {
    setIsImporting(true)
    setMessage('')

    try {
      const result = await apiRequest<{ created: number; updated: number; skipped: number; errors: string[] }>(
        '/api/spells/import/open5e',
        auth.token,
        { method: 'POST' },
      )
      await loadSpells()
      setMessage(
        `Importação concluída. Criadas: ${result.created}. Atualizadas: ${result.updated}. Ignoradas: ${result.skipped}. Erros: ${result.errors.length}.`,
      )
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao importar magias.')
    } finally {
      setIsImporting(false)
    }
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
        <div>
          <p className="text-sm font-semibold uppercase text-emerald-700 dark:text-emerald-400">Magias</p>
          <h2 className="mt-2 text-3xl font-semibold tracking-tight">Biblioteca de Magias</h2>
          <p className="mt-2 max-w-2xl text-slate-600 dark:text-slate-300">
            Cadastre magias próprias, filtre por nível/escola/classe e separe conteúdo privado ou de campanha.
          </p>
        </div>
        <div className="flex flex-col gap-3 sm:flex-row">
          {auth.user?.profile === 'GameMaster' && (
            <button
              className="inline-flex items-center justify-center gap-2 rounded-md border border-slate-300 px-4 py-3 text-sm font-semibold text-slate-700 disabled:opacity-60 dark:border-slate-700 dark:text-slate-200"
              disabled={isImporting}
              onClick={importOpen5e}
              type="button"
            >
              <RefreshCw className={isImporting ? 'animate-spin' : ''} size={18} />
              {isImporting ? 'Importando...' : 'Importar magias SRD'}
            </button>
          )}
          <button
            className="inline-flex items-center justify-center gap-2 rounded-md bg-emerald-600 px-4 py-3 text-sm font-semibold text-white"
            onClick={() => startNewSpell()}
            type="button"
          >
            <Plus size={18} />
            Nova magia
          </button>
        </div>
      </div>

      {message && (
        <p className="rounded-md bg-emerald-50 p-3 text-sm text-emerald-800 dark:bg-emerald-950 dark:text-emerald-200">
          {message}
        </p>
      )}

      <section className="grid gap-4 xl:grid-cols-[minmax(0,1fr)_420px]">
        <div className="space-y-4">
          <form
            className="grid gap-3 rounded-lg border border-slate-200 bg-white p-4 dark:border-slate-800 dark:bg-slate-900 md:grid-cols-4"
            onSubmit={(event) => {
              event.preventDefault()
              loadSpells({ ...filters, page: 1 })
            }}
          >
            <TextField label="Busca" onChange={(value) => updateFilter('name', value)} value={filters.name} />
            <label className="block">
              <span className="text-sm font-medium text-slate-700 dark:text-slate-200">Nível</span>
              <select
                className="mt-1 min-h-11 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
                onChange={(event) => updateFilter('level', event.target.value)}
                value={filters.level}
              >
                <option value="">Todos</option>
                {Array.from({ length: 10 }, (_, index) => (
                  <option key={index} value={index}>{index === 0 ? 'Truque' : index}</option>
                ))}
              </select>
            </label>
            <SpellSelect label="Escola" onChange={(value) => updateFilter('school', value)} value={filters.school} />
            <TextField label="Classe" onChange={(value) => updateFilter('className', value)} value={filters.className} />
            <TextField label="Fonte" onChange={(value) => updateFilter('source', value)} value={filters.source} />
            <SimpleSelect
              label="Concentração"
              onChange={(value) => updateFilter('isConcentration', value)}
              options={[['', 'Todas'], ['true', 'Sim'], ['false', 'Não']]}
              value={filters.isConcentration}
            />
            <SimpleSelect
              label="Ritual"
              onChange={(value) => updateFilter('isRitual', value)}
              options={[['', 'Todos'], ['true', 'Sim'], ['false', 'Não']]}
              value={filters.isRitual}
            />
            <SimpleSelect
              label="Visibilidade"
              onChange={(value) => updateFilter('visibility', value)}
              options={[['', 'Todas'], ['Private', 'Privada'], ['Campaign', 'Campanha'], ['LocalPublic', 'Pública local']]}
              value={filters.visibility}
            />
            <SimpleSelect
              label="Homebrew"
              onChange={(value) => updateFilter('isHomebrew', value)}
              options={[['', 'Todos'], ['true', 'Sim'], ['false', 'Não']]}
              value={filters.isHomebrew}
            />
            <button
              className="self-end rounded-md border border-slate-300 px-4 py-2.5 text-sm font-semibold text-slate-700 dark:border-slate-700 dark:text-slate-200 md:col-span-3"
              type="submit"
            >
              Filtrar
            </button>
          </form>

          {isLoading ? (
            <PanelText>Carregando magias...</PanelText>
          ) : !spells || spells.items.length === 0 ? (
            <PanelText>Nenhuma magia encontrada. Crie uma magia ou ajuste filtros.</PanelText>
          ) : (
            <div className="grid gap-3 lg:grid-cols-2">
              {spells.items.map((spell) => (
                <button
                  className="rounded-lg border border-slate-200 bg-white p-4 text-left shadow-sm transition hover:border-emerald-300 dark:border-slate-800 dark:bg-slate-900 dark:hover:border-emerald-700"
                  key={spell.id}
                  onClick={() => selectSpell(spell.id)}
                  type="button"
                >
                  <div className="flex items-start justify-between gap-3">
                    <div>
                      <h3 className="text-lg font-semibold">{spell.name}</h3>
                      <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">
                        {spell.level === 0 ? 'Truque' : `Nível ${spell.level}`} - {spell.school}
                      </p>
                    </div>
                    <span className="rounded-full bg-slate-100 px-2.5 py-1 text-xs font-semibold text-slate-700 dark:bg-slate-800 dark:text-slate-200">
                      {spellVisibilityLabel(spell.visibility)}
                    </span>
                  </div>
                  <p className="mt-3 line-clamp-2 text-sm leading-6 text-slate-600 dark:text-slate-300">
                    {spell.description || 'Sem descrição.'}
                  </p>
                  <div className="mt-3 flex flex-wrap gap-2 text-xs text-slate-500 dark:text-slate-400">
                    {spell.isConcentration && <span>Concentração</span>}
                    {spell.isRitual && <span>Ritual</span>}
                    {spell.availableClasses && <span>{spell.availableClasses}</span>}
                  </div>
                </button>
              ))}
            </div>
          )}

          {spells && spells.totalPages > 1 && (
            <div className="flex items-center justify-between rounded-lg border border-slate-200 bg-white p-3 text-sm dark:border-slate-800 dark:bg-slate-900">
              <button
                className="rounded-md border border-slate-300 px-3 py-2 disabled:opacity-50 dark:border-slate-700"
                disabled={filters.page <= 1}
                onClick={() => updateFilter('page', filters.page - 1)}
                type="button"
              >
                Anterior
              </button>
              <span>{spells.page} / {spells.totalPages} - {spells.totalItems} magia(s)</span>
              <button
                className="rounded-md border border-slate-300 px-3 py-2 disabled:opacity-50 dark:border-slate-700"
                disabled={filters.page >= spells.totalPages}
                onClick={() => updateFilter('page', filters.page + 1)}
                type="button"
              >
                Próxima
              </button>
            </div>
          )}
        </div>

        <aside className="space-y-4">
          {selectedSpell && !editingSpellId && (
            <SpellDetail spell={selectedSpell} onDelete={deleteSpell} onEdit={startEditSpell} />
          )}

          <form
            className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900"
            onSubmit={saveSpell}
          >
            <div className="flex items-center justify-between gap-3">
              <h3 className="text-lg font-semibold">{editingSpellId ? 'Editar magia' : 'Criar magia'}</h3>
              <div className="flex items-center gap-3">
                {isSpellFormOpen && (
                  <button className="text-sm font-medium text-slate-500 dark:text-slate-400" onClick={() => startNewSpell()} type="button">
                    Limpar
                  </button>
                )}
                <button
                  aria-label={isSpellFormOpen ? 'Recolher formulário de magia' : 'Expandir formulário de magia'}
                  className="rounded-md border border-slate-300 p-2 text-slate-600 transition hover:border-emerald-300 hover:text-emerald-700 dark:border-slate-700 dark:text-slate-300 dark:hover:border-emerald-700 dark:hover:text-emerald-300"
                  onClick={() => setIsSpellFormOpen((current) => !current)}
                  type="button"
                >
                  {isSpellFormOpen ? <ChevronUp size={18} /> : <ChevronDown size={18} />}
                </button>
              </div>
            </div>
            {isSpellFormOpen && (
              <div className="transition-all duration-200">
                <div className="mt-3 flex flex-wrap gap-2">
                  {spellExamples.map((example) => (
                    <button
                      className="rounded-md border border-slate-300 px-3 py-2 text-xs font-semibold text-slate-700 dark:border-slate-700 dark:text-slate-200"
                      key={example.name}
                      onClick={() => startNewSpell(example)}
                      type="button"
                    >
                      Exemplo: {example.name}
                    </button>
                  ))}
                </div>
                <div className="mt-4 grid gap-3">
                  <TextField label="Nome" onChange={(value) => patchDraft({ name: value })} required value={draft.name} />
                  <TextField label="Nome em inglês" onChange={(value) => patchDraft({ englishName: value })} value={draft.englishName} />
                  <div className="grid gap-3 sm:grid-cols-2">
                    <NumberField label="Nível" min={0} onChange={(value) => patchDraft({ level: value })} value={draft.level} />
                    <SpellSelect label="Escola" onChange={(value) => patchDraft({ school: value })} value={draft.school} />
                  </div>
                  <TextField label="Tempo de conjuração" onChange={(value) => patchDraft({ castingTime: value })} value={draft.castingTime} />
                  <TextField label="Alcance" onChange={(value) => patchDraft({ range: value })} value={draft.range} />
                  <TextField label="Componentes" onChange={(value) => patchDraft({ components: value })} value={draft.components} />
                  <TextField label="Material" onChange={(value) => patchDraft({ material: value })} value={draft.material} />
                  <TextField label="Duração" onChange={(value) => patchDraft({ duration: value })} value={draft.duration} />
                  <TextField label="Classes disponíveis" onChange={(value) => patchDraft({ availableClasses: value })} value={draft.availableClasses} />
                  <TextField label="Fonte" onChange={(value) => patchDraft({ source: value })} value={draft.source} />
                  <TextAreaField label="Descrição" onChange={(value) => patchDraft({ description: value })} value={draft.description} />
                  <TextAreaField label="Em níveis superiores" onChange={(value) => patchDraft({ higherLevelDescription: value })} value={draft.higherLevelDescription} />
                  <div className="grid gap-3 sm:grid-cols-2">
                    <label className="flex items-center gap-2 text-sm">
                      <input checked={draft.isConcentration} className="size-4" onChange={(event) => patchDraft({ isConcentration: event.target.checked })} type="checkbox" />
                      Concentração
                    </label>
                    <label className="flex items-center gap-2 text-sm">
                      <input checked={draft.isRitual} className="size-4" onChange={(event) => patchDraft({ isRitual: event.target.checked })} type="checkbox" />
                      Ritual
                    </label>
                    <label className="flex items-center gap-2 text-sm">
                      <input checked={draft.isHomebrew} className="size-4" onChange={(event) => patchDraft({ isHomebrew: event.target.checked })} type="checkbox" />
                      Homebrew
                    </label>
                  </div>
                  <SimpleSelect
                    label="Visibilidade"
                    onChange={(value) => patchDraft({ visibility: value as SpellVisibility })}
                    options={visibilityOptions}
                    value={draft.visibility}
                  />
                  {draft.visibility === 'Campaign' && (
                    <label className="block">
                      <span className="text-sm font-medium text-slate-700 dark:text-slate-200">Campanha</span>
                      <select
                        className="mt-1 min-h-11 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
                        onChange={(event) => patchDraft({ campaignId: event.target.value || null })}
                        required
                        value={draft.campaignId ?? ''}
                      >
                        <option value="">Selecione campanha</option>
                        {masterCampaigns.map((campaign) => (
                          <option key={campaign.id} value={campaign.id}>{campaign.name}</option>
                        ))}
                      </select>
                    </label>
                  )}
                  <button
                    className="inline-flex items-center justify-center gap-2 rounded-md bg-emerald-600 px-4 py-3 text-sm font-semibold text-white disabled:bg-slate-400"
                    disabled={isSaving}
                    type="submit"
                  >
                    <Plus size={18} />
                    {isSaving ? 'Salvando...' : 'Salvar magia'}
                  </button>
                </div>
              </div>
            )}
          </form>
        </aside>
      </section>
    </div>
  )
}

function SpellDetail({
  onDelete,
  onEdit,
  spell,
}: {
  onDelete: (spellId: string) => void
  onEdit: (spell: SpellResponse) => void
  spell: SpellResponse
}) {
  return (
    <div className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
      <div className="flex items-start justify-between gap-3">
        <div>
          <p className="text-sm font-semibold uppercase text-emerald-700 dark:text-emerald-400">
            {spell.level === 0 ? 'Truque' : `Nível ${spell.level}`} - {spell.school}
          </p>
          <h3 className="mt-2 text-2xl font-semibold">{spell.name}</h3>
          {spell.englishName && <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">{spell.englishName}</p>}
        </div>
        {spell.canEdit && (
          <div className="flex gap-2">
            <button className="rounded-md border border-slate-300 p-2 dark:border-slate-700" onClick={() => onEdit(spell)} type="button" aria-label="Editar magia">
              <Edit size={17} />
            </button>
            <button className="rounded-md border border-red-300 p-2 text-red-700 dark:border-red-900 dark:text-red-300" onClick={() => onDelete(spell.id)} type="button" aria-label="Excluir magia">
              <Trash2 size={17} />
            </button>
          </div>
        )}
      </div>
      <div className="mt-4 grid gap-2 text-sm text-slate-600 dark:text-slate-300">
        <InfoRow label="Tempo" value={spell.castingTime} />
        <InfoRow label="Alcance" value={spell.range} />
        <InfoRow label="Componentes" value={spell.components} />
        <InfoRow label="Material" value={spell.material} />
        <InfoRow label="Duração" value={spell.duration} />
        <InfoRow label="Classes" value={spell.availableClasses} />
        <InfoRow label="Fonte" value={spell.source} />
        <InfoRow label="Visibilidade" value={spellVisibilityLabel(spell.visibility)} />
        {spell.campaignName && <InfoRow label="Campanha" value={spell.campaignName} />}
      </div>
      <div className="mt-4 flex flex-wrap gap-2 text-xs font-semibold">
        {spell.isConcentration && <span className="rounded-full bg-amber-100 px-2 py-1 text-amber-800 dark:bg-amber-950 dark:text-amber-200">Concentração</span>}
        {spell.isRitual && <span className="rounded-full bg-sky-100 px-2 py-1 text-sky-800 dark:bg-sky-950 dark:text-sky-200">Ritual</span>}
        {spell.isHomebrew && <span className="rounded-full bg-emerald-100 px-2 py-1 text-emerald-800 dark:bg-emerald-950 dark:text-emerald-200">Homebrew</span>}
      </div>
      <TextBlock label="Descrição" value={spell.description} />
      <TextBlock label="Em níveis superiores" value={spell.higherLevelDescription} />
    </div>
  )
}

function SpellSelect({ label, onChange, value }: { label: string; onChange: (value: string) => void; value: string }) {
  return (
    <label className="block">
      <span className="text-sm font-medium text-slate-700 dark:text-slate-200">{label}</span>
      <select
        className="mt-1 min-h-11 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
        onChange={(event) => onChange(event.target.value)}
        value={value}
      >
        <option value="">Todas</option>
        {spellSchools.map((school) => (
          <option key={school} value={school}>{school}</option>
        ))}
      </select>
    </label>
  )
}

function SimpleSelect({
  disabled,
  label,
  onChange,
  options,
  value,
}: {
  disabled?: boolean
  label: string
  onChange: (value: string) => void
  options: [string, string][]
  value: string
}) {
  return (
    <label className="block">
      <span className="text-sm font-medium text-slate-700 dark:text-slate-200">{label}</span>
      <select
        className="mt-1 min-h-11 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 disabled:opacity-70 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
        disabled={disabled}
        onChange={(event) => onChange(event.target.value)}
        value={value}
      >
        {options.map(([optionValue, labelText]) => (
          <option key={optionValue} value={optionValue}>{labelText}</option>
        ))}
      </select>
    </label>
  )
}

function toSpellPayload(spell: SpellResponse): SpellPayload {
  return {
    name: spell.name,
    englishName: spell.englishName,
    level: spell.level,
    school: spell.school,
    castingTime: spell.castingTime,
    range: spell.range,
    components: spell.components,
    material: spell.material,
    duration: spell.duration,
    isConcentration: spell.isConcentration,
    isRitual: spell.isRitual,
    description: spell.description,
    higherLevelDescription: spell.higherLevelDescription,
    availableClasses: spell.availableClasses,
    source: spell.source,
    isHomebrew: spell.isHomebrew,
    visibility: spell.visibility,
    campaignId: spell.campaignId ?? null,
  }
}

function normalizeSpellPayload(payload: SpellPayload): SpellPayload {
  return {
    ...payload,
    level: Number(payload.level),
    campaignId: payload.visibility === 'Campaign' ? payload.campaignId || null : null,
  }
}

function spellVisibilityLabel(visibility: SpellVisibility) {
  return visibility === 'Private' ? 'Privada' : visibility === 'Campaign' ? 'Campanha' : 'Pública local'
}

const featureTypeOptions: { value: FeatureType; label: string }[] = [
  { value: 'Feat', label: 'Talento' },
  { value: 'Class', label: 'Classe' },
  { value: 'Subclass', label: 'Subclasse' },
  { value: 'Species', label: 'Espécie' },
  { value: 'Background', label: 'Antecedente' },
  { value: 'Homebrew', label: 'Homebrew' },
]

const recoveryTypeOptions: [string, string][] = [
  ['Manual', 'Manual'],
  ['ShortRest', 'Descanso curto'],
  ['LongRest', 'Descanso longo'],
]

const featureExamples: FeaturePayload[] = [
  {
    name: 'Olhar de Cartógrafo',
    type: 'Feat',
    description: 'Você memoriza rotas, marcas naturais e atalhos urbanos com facilidade incomum.',
    source: 'Exemplo fictício local',
    prerequisites: 'Treinamento em Sobrevivência ou História',
    isHomebrew: true,
    visibility: 'Private',
    campaignId: null,
  },
  {
    name: 'Eco da Linhagem Antiga',
    type: 'Species',
    description: 'Uma vez por descanso, você recebe vantagem em um teste ligado à memória ancestral da sua espécie.',
    source: 'Exemplo fictício local',
    prerequisites: '',
    isHomebrew: true,
    visibility: 'Private',
    campaignId: null,
  },
]

const emptyFeaturePayload: FeaturePayload = {
  name: '',
  type: 'Feat',
  description: '',
  source: 'Homebrew local',
  prerequisites: '',
  isHomebrew: true,
  visibility: 'Private',
  campaignId: null,
}

function FeaturesPage({ auth }: { auth: AuthContextValue }) {
  const [features, setFeatures] = useState<PagedResponse<FeatureResponse> | null>(null)
  const [campaigns, setCampaigns] = useState<CampaignSummaryResponse[]>([])
  const [selectedFeature, setSelectedFeature] = useState<FeatureResponse | null>(null)
  const [editingFeatureId, setEditingFeatureId] = useState<string | null>(null)
  const [draft, setDraft] = useState<FeaturePayload>(emptyFeaturePayload)
  const [message, setMessage] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)
  const [isFormOpen, setIsFormOpen] = useState(false)
  const [filters, setFilters] = useState({
    name: '',
    type: '',
    source: '',
    isHomebrew: '',
    visibility: '',
    page: 1,
    pageSize: 12,
  })

  const masterCampaigns = campaigns.filter((campaign) => campaign.currentUserRole === 'Master')
  const visibilityOptions = contentVisibilityOptions(masterCampaigns.length > 0)

  useEffect(() => {
    apiRequest<CampaignSummaryResponse[]>('/api/campaigns', auth.token)
      .then(setCampaigns)
      .catch(() => setCampaigns([]))
  }, [auth.token])

  useEffect(() => {
    loadFeatures()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [auth.token, filters.page])

  async function loadFeatures(nextFilters = filters) {
    setIsLoading(true)
    setMessage('')

    const query = new URLSearchParams()
    Object.entries(nextFilters).forEach(([key, value]) => {
      if (value !== '' && value !== null && value !== undefined) {
        query.set(key, String(value))
      }
    })

    try {
      setFeatures(await apiRequest<PagedResponse<FeatureResponse>>(`/api/features?${query.toString()}`, auth.token))
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao carregar talentos.')
    } finally {
      setIsLoading(false)
    }
  }

  function updateFilter(key: keyof typeof filters, value: string | number) {
    setFilters((current) => ({ ...current, [key]: value, page: key === 'page' ? Number(value) : 1 }))
  }

  function startNewFeature(example?: FeaturePayload) {
    setEditingFeatureId(null)
    setSelectedFeature(null)
    const next = { ...(example ?? emptyFeaturePayload) }
    setDraft(masterCampaigns.length > 0 ? next : { ...next, visibility: next.visibility === 'Campaign' ? 'Private' : next.visibility, campaignId: null })
    setMessage('')
    setIsFormOpen(true)
  }

  function startEditFeature(feature: FeatureResponse) {
    setEditingFeatureId(feature.id)
    setSelectedFeature(feature)
    setDraft(toFeaturePayload(feature))
    setMessage('')
    setIsFormOpen(true)
  }

  function patchDraft(patch: Partial<FeaturePayload>) {
    setDraft((current) => {
      const next = { ...current, ...patch }
      if (next.visibility !== 'Campaign') {
        next.campaignId = null
      }
      return next
    })
  }

  async function selectFeature(id: string) {
    setMessage('')
    try {
      setSelectedFeature(await apiRequest<FeatureResponse>(`/api/features/${id}`, auth.token))
      setEditingFeatureId(null)
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao abrir talento.')
    }
  }

  async function saveFeature(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setIsSaving(true)
    setMessage('')

    try {
      const feature = await apiRequest<FeatureResponse>(
        editingFeatureId ? `/api/features/${editingFeatureId}` : '/api/features',
        auth.token,
        {
          method: editingFeatureId ? 'PUT' : 'POST',
          body: JSON.stringify(normalizeFeaturePayload(draft)),
        },
      )
      setSelectedFeature(feature)
      setEditingFeatureId(null)
      setDraft(emptyFeaturePayload)
      setIsFormOpen(false)
      await loadFeatures()
      setMessage('Talento/característica salvo.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao salvar talento.')
    } finally {
      setIsSaving(false)
    }
  }

  async function deleteFeature(featureId: string) {
    setIsSaving(true)
    setMessage('')

    try {
      await apiRequest<null>(`/api/features/${featureId}`, auth.token, { method: 'DELETE' })
      setSelectedFeature(null)
      setEditingFeatureId(null)
      await loadFeatures()
      setMessage('Talento/característica excluído.')
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Erro ao excluir talento.')
    } finally {
      setIsSaving(false)
    }
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
        <div>
          <p className="text-sm font-semibold uppercase text-emerald-700 dark:text-emerald-400">Talentos</p>
          <h2 className="mt-2 text-3xl font-semibold tracking-tight">Biblioteca de Talentos e Características</h2>
          <p className="mt-2 max-w-2xl text-slate-600 dark:text-slate-300">
            Cadastre talentos, traços de classe, espécie, antecedente e conteúdo homebrew sem copiar material protegido.
          </p>
        </div>
        <button className="inline-flex items-center justify-center gap-2 rounded-md bg-emerald-600 px-4 py-3 text-sm font-semibold text-white" onClick={() => startNewFeature()} type="button">
          <Plus size={18} />
          Novo talento
        </button>
      </div>

      {message && <p className="rounded-md bg-emerald-50 p-3 text-sm text-emerald-800 dark:bg-emerald-950 dark:text-emerald-200">{message}</p>}

      <section className="grid gap-4 xl:grid-cols-[minmax(0,1fr)_420px]">
        <div className="space-y-4">
          <form
            className="grid gap-3 rounded-lg border border-slate-200 bg-white p-4 dark:border-slate-800 dark:bg-slate-900 md:grid-cols-3"
            onSubmit={(event) => {
              event.preventDefault()
              loadFeatures({ ...filters, page: 1 })
            }}
          >
            <TextField label="Busca" onChange={(value) => updateFilter('name', value)} value={filters.name} />
            <FeatureTypeSelect label="Tipo" includeAll onChange={(value) => updateFilter('type', value)} value={filters.type} />
            <TextField label="Fonte" onChange={(value) => updateFilter('source', value)} value={filters.source} />
            <SimpleSelect label="Homebrew" onChange={(value) => updateFilter('isHomebrew', value)} options={[['', 'Todos'], ['true', 'Sim'], ['false', 'Não']]} value={filters.isHomebrew} />
            <SimpleSelect label="Visibilidade" onChange={(value) => updateFilter('visibility', value)} options={[['', 'Todas'], ['Private', 'Privada'], ['Campaign', 'Campanha'], ['LocalPublic', 'Pública local']]} value={filters.visibility} />
            <button className="self-end rounded-md border border-slate-300 px-4 py-2.5 text-sm font-semibold text-slate-700 dark:border-slate-700 dark:text-slate-200" type="submit">
              Filtrar
            </button>
          </form>

          {isLoading ? (
            <PanelText>Carregando talentos...</PanelText>
          ) : !features || features.items.length === 0 ? (
            <PanelText>Nenhum talento ou característica encontrado.</PanelText>
          ) : (
            <div className="grid gap-3 lg:grid-cols-2">
              {features.items.map((feature) => (
                <button className="rounded-lg border border-slate-200 bg-white p-4 text-left shadow-sm transition hover:border-emerald-300 dark:border-slate-800 dark:bg-slate-900 dark:hover:border-emerald-700" key={feature.id} onClick={() => selectFeature(feature.id)} type="button">
                  <div className="flex items-start justify-between gap-3">
                    <div>
                      <h3 className="text-lg font-semibold">{feature.name}</h3>
                      <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">{featureTypeLabel(feature.type)} - {feature.source || 'Sem fonte'}</p>
                    </div>
                    <span className="rounded-full bg-slate-100 px-2.5 py-1 text-xs font-semibold text-slate-700 dark:bg-slate-800 dark:text-slate-200">{spellVisibilityLabel(feature.visibility)}</span>
                  </div>
                  <p className="mt-3 line-clamp-2 text-sm leading-6 text-slate-600 dark:text-slate-300">{feature.description || 'Sem descrição.'}</p>
                </button>
              ))}
            </div>
          )}

          {features && features.totalPages > 1 && (
            <div className="flex items-center justify-between rounded-lg border border-slate-200 bg-white p-3 text-sm dark:border-slate-800 dark:bg-slate-900">
              <button className="rounded-md border border-slate-300 px-3 py-2 disabled:opacity-50 dark:border-slate-700" disabled={filters.page <= 1} onClick={() => updateFilter('page', filters.page - 1)} type="button">
                Anterior
              </button>
              <span>{features.page} / {features.totalPages} - {features.totalItems} item(ns)</span>
              <button className="rounded-md border border-slate-300 px-3 py-2 disabled:opacity-50 dark:border-slate-700" disabled={filters.page >= features.totalPages} onClick={() => updateFilter('page', filters.page + 1)} type="button">
                Próxima
              </button>
            </div>
          )}
        </div>

        <aside className="space-y-4">
          {selectedFeature && !editingFeatureId && (
            <FeatureDetail feature={selectedFeature} onDelete={deleteFeature} onEdit={startEditFeature} />
          )}

          <form className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900" onSubmit={saveFeature}>
            <div className="flex items-center justify-between gap-3">
              <h3 className="text-lg font-semibold">{editingFeatureId ? 'Editar' : 'Criar'} talento/característica</h3>
              <div className="flex items-center gap-3">
                {isFormOpen && (
                  <button className="text-sm font-medium text-slate-500 dark:text-slate-400" onClick={() => startNewFeature()} type="button">
                    Limpar
                  </button>
                )}
                <button aria-label={isFormOpen ? 'Recolher formulário' : 'Expandir formulário'} className="rounded-md border border-slate-300 p-2 text-slate-600 dark:border-slate-700 dark:text-slate-300" onClick={() => setIsFormOpen((current) => !current)} type="button">
                  {isFormOpen ? <ChevronUp size={18} /> : <ChevronDown size={18} />}
                </button>
              </div>
            </div>
            {isFormOpen && (
              <div className="mt-4 grid gap-3">
                <div className="flex flex-wrap gap-2">
                  {featureExamples.map((example) => (
                    <button className="rounded-md border border-slate-300 px-3 py-2 text-xs font-semibold text-slate-700 dark:border-slate-700 dark:text-slate-200" key={example.name} onClick={() => startNewFeature(example)} type="button">
                      Exemplo: {example.name}
                    </button>
                  ))}
                </div>
                <TextField label="Nome" onChange={(value) => patchDraft({ name: value })} required value={draft.name} />
                <FeatureTypeSelect label="Tipo" onChange={(value) => patchDraft({ type: value as FeatureType })} value={draft.type} />
                <TextField label="Fonte" onChange={(value) => patchDraft({ source: value })} value={draft.source} />
                <TextField label="Pré-requisitos" onChange={(value) => patchDraft({ prerequisites: value })} value={draft.prerequisites} />
                <TextAreaField label="Descrição" onChange={(value) => patchDraft({ description: value })} value={draft.description} />
                <label className="flex items-center gap-2 text-sm">
                  <input checked={draft.isHomebrew} className="size-4" onChange={(event) => patchDraft({ isHomebrew: event.target.checked })} type="checkbox" />
                  Homebrew
                </label>
                <SimpleSelect label="Visibilidade" onChange={(value) => patchDraft({ visibility: value as SpellVisibility })} options={visibilityOptions} value={draft.visibility} />
                {draft.visibility === 'Campaign' && (
                  <label className="block">
                    <span className="text-sm font-medium text-slate-700 dark:text-slate-200">Campanha</span>
                    <select className="mt-1 min-h-11 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100" onChange={(event) => patchDraft({ campaignId: event.target.value || null })} required value={draft.campaignId ?? ''}>
                      <option value="">Selecione campanha</option>
                      {masterCampaigns.map((campaign) => (
                        <option key={campaign.id} value={campaign.id}>{campaign.name}</option>
                      ))}
                    </select>
                  </label>
                )}
                <button className="inline-flex items-center justify-center gap-2 rounded-md bg-emerald-600 px-4 py-3 text-sm font-semibold text-white disabled:bg-slate-400" disabled={isSaving} type="submit">
                  <Plus size={18} />
                  {isSaving ? 'Salvando...' : 'Salvar'}
                </button>
              </div>
            )}
          </form>
        </aside>
      </section>
    </div>
  )
}

function FeatureDetail({
  feature,
  onDelete,
  onEdit,
}: {
  feature: FeatureResponse
  onDelete: (featureId: string) => void
  onEdit: (feature: FeatureResponse) => void
}) {
  return (
    <div className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
      <div className="flex items-start justify-between gap-3">
        <div>
          <p className="text-sm font-semibold uppercase text-emerald-700 dark:text-emerald-400">{featureTypeLabel(feature.type)}</p>
          <h3 className="mt-2 text-2xl font-semibold">{feature.name}</h3>
        </div>
        {feature.canEdit && (
          <div className="flex gap-2">
            <button className="rounded-md border border-slate-300 p-2 dark:border-slate-700" onClick={() => onEdit(feature)} type="button" aria-label="Editar">
              <Edit size={17} />
            </button>
            <button className="rounded-md border border-red-300 p-2 text-red-700 dark:border-red-900 dark:text-red-300" onClick={() => onDelete(feature.id)} type="button" aria-label="Excluir">
              <Trash2 size={17} />
            </button>
          </div>
        )}
      </div>
      <div className="mt-4 grid gap-2 text-sm text-slate-600 dark:text-slate-300">
        <InfoRow label="Fonte" value={feature.source} />
        <InfoRow label="Pré-requisitos" value={feature.prerequisites} />
        <InfoRow label="Visibilidade" value={spellVisibilityLabel(feature.visibility)} />
        {feature.campaignName && <InfoRow label="Campanha" value={feature.campaignName} />}
      </div>
      {feature.isHomebrew && <span className="mt-4 inline-flex rounded-full bg-emerald-100 px-2 py-1 text-xs font-semibold text-emerald-800 dark:bg-emerald-950 dark:text-emerald-200">Homebrew</span>}
      <TextBlock label="Descrição" value={feature.description} />
    </div>
  )
}

function FeatureTypeSelect({
  includeAll,
  label,
  onChange,
  value,
}: {
  includeAll?: boolean
  label: string
  onChange: (value: string) => void
  value: string
}) {
  return (
    <label className="block">
      <span className="text-sm font-medium text-slate-700 dark:text-slate-200">{label}</span>
      <select className="mt-1 min-h-11 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100" onChange={(event) => onChange(event.target.value)} value={value}>
        {includeAll && <option value="">Todos</option>}
        {featureTypeOptions.map((option) => (
          <option key={option.value} value={option.value}>{option.label}</option>
        ))}
      </select>
    </label>
  )
}

function toFeaturePayload(feature: FeatureResponse): FeaturePayload {
  return {
    name: feature.name,
    type: feature.type,
    description: feature.description,
    source: feature.source,
    prerequisites: feature.prerequisites,
    isHomebrew: feature.isHomebrew,
    visibility: feature.visibility,
    campaignId: feature.campaignId ?? null,
  }
}

function normalizeFeaturePayload(payload: FeaturePayload): FeaturePayload {
  return {
    ...payload,
    campaignId: payload.visibility === 'Campaign' ? payload.campaignId || null : null,
  }
}

function featureTypeLabel(type: FeatureType) {
  return featureTypeOptions.find((option) => option.value === type)?.label ?? type
}

function StatCard({
  icon: Icon,
  label,
  value,
}: {
  icon: typeof Swords
  label: string
  value: string
}) {
  return (
    <div className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
      <Icon className="mb-4 text-emerald-700 dark:text-emerald-300" size={22} />
      <p className="text-sm text-slate-500 dark:text-slate-400">{label}</p>
      <p className="mt-1 text-xl font-semibold">{value}</p>
    </div>
  )
}

function InfoPanel({ children, title }: { children: ReactNode; title: string }) {
  return (
    <div className="rounded-lg border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
      <h3 className="text-lg font-semibold">{title}</h3>
      <div className="mt-4 space-y-3">{children}</div>
    </div>
  )
}

function InfoRow({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex items-start justify-between gap-4 text-sm">
      <span className="text-slate-500 dark:text-slate-400">{label}</span>
      <span className="text-right font-medium text-slate-800 dark:text-slate-100">{value || '-'}</span>
    </div>
  )
}

function TextBlock({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <p className="text-sm font-medium text-slate-700 dark:text-slate-200">{label}</p>
      <p className="mt-1 whitespace-pre-wrap text-sm leading-6 text-slate-600 dark:text-slate-300">
        {value || '-'}
      </p>
    </div>
  )
}

function toCharacterPayload(character: CharacterResponse): CharacterPayload {
  return {
    campaignId: character.campaignId ?? null,
    name: character.name,
    nickname: character.nickname ?? '',
    avatarUrl: character.avatarUrl ?? '',
    tokenImageUrl: character.tokenImageUrl ?? '',
    totalLevel: character.totalLevel,
    species: character.species,
    mainClass: character.mainClass,
    subclass: character.subclass,
    background: character.background,
    alignment: character.alignment,
    experience: character.experience,
    inspiration: character.inspiration,
    proficiencyBonus: character.proficiencyBonus,
    armorClass: character.armorClass,
    initiative: character.initiative,
    speed: character.speed,
    maxHitPoints: character.maxHitPoints,
    currentHitPoints: character.currentHitPoints,
    temporaryHitPoints: character.temporaryHitPoints,
    totalHitDice: character.totalHitDice,
    availableHitDice: character.availableHitDice,
    physicalDescription: character.physicalDescription,
    personalityTraits: character.personalityTraits,
    ideals: character.ideals,
    bonds: character.bonds,
    flaws: character.flaws,
    backstory: character.backstory,
    quickNotes: character.quickNotes,
  }
}

function normalizeCharacterPayload(payload: CharacterPayload): CharacterPayload {
  return {
    ...payload,
    campaignId: payload.campaignId || null,
    nickname: payload.nickname || null,
    avatarUrl: payload.avatarUrl || null,
    tokenImageUrl: payload.tokenImageUrl || null,
  }
}

function TextAreaField({
  label,
  onChange,
  value,
}: {
  label: string
  onChange: (value: string) => void
  value: string
}) {
  return (
    <label className="block">
      <span className="text-sm font-medium text-slate-700 dark:text-slate-200">{label}</span>
      <textarea
        className="mt-1 min-h-32 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-slate-950 shadow-sm outline-none transition placeholder:text-slate-400 focus:border-emerald-600 focus:ring-2 focus:ring-emerald-500/20 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
        onChange={(event) => onChange(event.target.value)}
        value={value}
      />
    </label>
  )
}

function RoleBadge({ role }: { role: CampaignRole }) {
  return (
    <span className="inline-flex rounded-full bg-slate-100 px-2.5 py-1 text-xs font-semibold text-slate-700 dark:bg-slate-800 dark:text-slate-200">
      {role === 'Master' ? 'Mestre' : 'Jogador'}
    </span>
  )
}

function BackLink({ to }: { to: string }) {
  return (
    <Link className="inline-flex min-h-11 items-center gap-2 rounded-md px-1 text-sm font-medium text-slate-600 transition hover:text-emerald-700 dark:text-slate-300 dark:hover:text-emerald-300" to={to}>
      <ArrowLeft size={17} />
      Voltar
    </Link>
  )
}

function PanelText({ children }: { children: ReactNode }) {
  return (
    <div className="mx-auto max-w-6xl rounded-lg border border-slate-200 bg-white p-4 text-slate-600 shadow-sm dark:border-slate-800 dark:bg-slate-900 dark:text-slate-300 sm:p-6">
      {children}
    </div>
  )
}

function AccessDenied({ backTo }: { backTo: string }) {
  return (
    <div className="space-y-4">
      <PanelText>
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h3 className="text-lg font-semibold text-slate-950 dark:text-white">Acesso negado</h3>
            <p className="mt-1 text-sm">Você não tem permissão para editar este recurso.</p>
          </div>
          <Link
            className="inline-flex items-center justify-center gap-2 rounded-md border border-slate-300 px-4 py-3 text-sm font-semibold text-slate-700 dark:border-slate-700 dark:text-slate-200"
            to={backTo}
          >
            <ArrowLeft size={17} />
            Voltar
          </Link>
        </div>
      </PanelText>
    </div>
  )
}

function LoadingScreen() {
  return (
    <main className="grid min-h-screen place-items-center bg-slate-100 text-slate-950 dark:bg-slate-950 dark:text-white">
      <div className="rounded-lg border border-slate-200 bg-white p-5 text-sm shadow-sm dark:border-slate-800 dark:bg-slate-900">
        Carregando sessão...
      </div>
    </main>
  )
}

export default App
