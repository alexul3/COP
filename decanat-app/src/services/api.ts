import axios, { AxiosInstance, AxiosResponse, AxiosError } from 'axios'

// Интерфейсы для типизации
export interface LoginRequest {
  username: string
  password: string
}

export interface UserInfo {
  id: number
  username: string
  role: 'Student' | 'Teacher' | 'DecanatWorker'
  studentId?: number
  teacherId?: number
  decanatWorkerId?: number
}

export interface ScheduleDto {
  id: number
  date: string
  pairNumber: number
  classroom: string
  group: {
    id: number
    name: string
  }
  subject: {
    id: number
    name: string
    description: string
  }
  teacher: {
    id: number
    name: string
  }
}

export interface ExamDto {
  id: number
  examScore: number
  subject: {
    id: number
    name: string
  }
  student: {
    id: number
    name: string
    group: {
      id: number
      name: string
    }
  }
  teacher: {
    id: number
    name: string
  }
}

export interface CreateExamDto {
  studentId: number
  teacherId: number
  subjectId: number
  examScore: number
}

export interface CreateScheduleDto {
  groupId: number
  teacherId: number
  subjectId: number
  date: string
  pairNumber: number
  classroom: string
}

export interface GroupDto {
  id: number
  name: string
}

export interface StudentDto {
  id: number
  name: string
  group: GroupDto | null // Добавьте group
}

// Основной API сервис
class ApiService {
  private api: AxiosInstance
  private baseURL = 'http://localhost:5191/api'

  constructor() {
    this.api = axios.create({
      baseURL: this.baseURL,
      timeout: 10000,
      headers: {
        'Content-Type': 'application/json'
      }
    })

    // Интерцептор для обработки ошибок
    this.api.interceptors.response.use(
      (response) => response,
      (error: AxiosError) => {
        console.error('API Error:', error.response?.data || error.message)
        return Promise.reject(error)
      }
    )
  }

  // Аутентификация
  async login(credentials: LoginRequest): Promise<UserInfo> {
    const response = await this.api.post<UserInfo>('/auth/login', credentials)
    localStorage.setItem('user', JSON.stringify(response.data))
    return response.data
  }

  async register(credentials: LoginRequest): Promise<UserInfo> {
    const response = await this.api.post<UserInfo>('/auth/register', credentials)
    return response.data
  }

  async getTeacherGrades(teacherId: number): Promise<ExamDto[]> {
  // Этот endpoint нужно добавить на бэкенде
  const response = await this.api.get<ExamDto[]>(`/teacher/${teacherId}/grades`)
  return response.data
  }

  // Получение всех оценок
  async getAllExams(): Promise<ExamDto[]> {
    const response = await this.api.get<ExamDto[]>('/exams')
    return response.data
  }

  // Обновление оценки
  async updateExam(id: number, exam: CreateExamDto): Promise<void> {
    await this.api.put(`/exams/${id}`, exam)
  }

  // Удаление оценки
  async deleteExam(id: number): Promise<void> {
    await this.api.delete(`/exams/${id}`)
  }

  // Получение текущего пользователя
  getCurrentUser(): UserInfo | null {
    const userStr = localStorage.getItem('user')
    return userStr ? JSON.parse(userStr) : null
  }

  logout(): void {
    localStorage.removeItem('user')
  }

  // Расписание
  async getStudentSchedule(studentId: number): Promise<ScheduleDto[]> {
    const response = await this.api.get<ScheduleDto[]>(`/student/${studentId}/schedule`)
    return response.data
  }

  async getTeacherSchedule(teacherId: number): Promise<ScheduleDto[]> {
    const response = await this.api.get<ScheduleDto[]>(`/teacher/${teacherId}/schedule`)
    return response.data
  }

  async getAllSchedules(): Promise<ScheduleDto[]> {
    const response = await this.api.get<ScheduleDto[]>('/decanat/schedules')
    return response.data
  }

  async createSchedule(schedule: CreateScheduleDto): Promise<ScheduleDto> {
    const response = await this.api.post<ScheduleDto>('/decanat/schedule', schedule)
    return response.data
  }

  async updateSchedule(id: number, schedule: CreateScheduleDto): Promise<void> {
    await this.api.put(`/decanat/schedule/${id}`, schedule)
  }

  async deleteSchedule(id: number): Promise<void> {
    await this.api.delete(`/decanat/schedule/${id}`)
  }

  async getStudentsWithGroups(): Promise<StudentDto[]> {
  const response = await this.api.get<StudentDto[]>('/teacher/students')
  return response.data
}

  // Оценки
  async getStudentGrades(studentId: number): Promise<ExamDto[]> {
    const response = await this.api.get<ExamDto[]>(`/student/${studentId}/grades`)
    return response.data
  }

  async addGrade(grade: CreateExamDto): Promise<ExamDto> {
    const response = await this.api.post<ExamDto>('/teacher/grade', grade)
    return response.data
  }

  // Справочники
  async getStudents(): Promise<any[]> {
    const response = await this.api.get('/teacher/students')
    return response.data
  }

  async getTeachers(): Promise<any[]> {
    const response = await this.api.get('/decanat/teachers')
    return response.data
  }

  async getGroups(): Promise<any[]> {
    const response = await this.api.get('/decanat/groups')
    return response.data
  }

  async getSubjects(): Promise<any[]> {
    const response = await this.api.get('/decanat/subjects')
    return response.data
  }
}

export default new ApiService()