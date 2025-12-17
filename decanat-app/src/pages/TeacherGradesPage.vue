<template>
  <div class="teacher-grades-page">
    <div class="page-header">
      <h2>Журнал оценок</h2>
      <button class="add-btn" @click="showAddGradeModal = true">
        + Выставить оценку
      </button>
    </div>

    <div class="filters">
      <div class="filter-group">
        <label>Студент:</label>
        <select v-model="selectedStudentId">
          <option value="">Все студенты</option>
          <option v-for="student in students" :key="student.id" :value="student.id">
            {{ student.name }} ({{ student.group?.name }})
          </option>
        </select>
      </div>
      
      <div class="filter-group">
        <label>Предмет:</label>
        <select v-model="selectedSubjectId">
          <option value="">Все предметы</option>
          <option v-for="subject in subjects" :key="subject.id" :value="subject.id">
            {{ subject.name }}
          </option>
        </select>
      </div>
      
      <button class="filter-btn" @click="loadData">
        Применить фильтр
      </button>
      <button class="reset-btn" @click="resetFilters">
        Сбросить
      </button>
    </div>

    <!-- Статистика -->
    <div class="stats" v-if="!isLoading && !error">
      <div class="stat-card">
        <div class="stat-value">{{ filteredGrades.length }}</div>
        <div class="stat-label">Всего оценок</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">{{ averageGrade }}</div>
        <div class="stat-label">Средний балл</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">{{ uniqueStudents }}</div>
        <div class="stat-label">Студентов</div>
      </div>
    </div>

    <div v-if="isLoading" class="loading">
      <div class="spinner"></div>
      <p>Загрузка данных...</p>
    </div>

    <div v-else-if="error" class="error">
      {{ error }}
    </div>

    <div v-else class="grades-container">
      <table class="grades-table">
        <thead>
          <tr>
            <th>Студент</th>
            <th>Группа</th>
            <th>Предмет</th>
            <th>Оценка</th>
            <th>Преподаватель</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="grade in filteredGrades" :key="grade.id">
            <td>{{ grade.student.name }}</td>
            <td>{{ grade.student.group?.name || 'Не указана' }}</td>
            <td>{{ grade.subject.name }}</td>
            <td>
              <span :class="['grade-badge', getGradeClass(grade.examScore)]">
                {{ grade.examScore }}
              </span>
            </td>
            <td>{{ grade.teacher.name }}</td>
            <td class="actions">
              <button class="edit-btn" @click="editGrade(grade)" title="Редактировать">
                ✏️
              </button>
              <button class="delete-btn" @click="deleteGrade(grade.id)" title="Удалить">
                🗑️
              </button>
            </td>
          </tr>
        </tbody>
      </table>

      <div v-if="filteredGrades.length === 0 && !isLoading && !error" class="empty-state">
        <p>Нет оценок для отображения</p>
        <button @click="showAddGradeModal = true" class="add-first-btn">
          Выставить первую оценку
        </button>
      </div>
    </div>

    <!-- Модальное окно добавления/редактирования оценки -->
    <div v-if="showAddGradeModal || editingGrade" class="modal-overlay" @click.self="closeModal">
      <div class="modal">
        <h3>{{ editingGrade ? 'Редактирование оценки' : 'Выставление новой оценки' }}</h3>
        <form @submit.prevent="saveGrade">
          <div class="form-group">
            <label>Студент *</label>
            <select v-model="gradeForm.studentId" required :disabled="!!editingGrade">
              <option value="">Выберите студента</option>
              <option 
                v-for="student in students" 
                :key="student.id" 
                :value="student.id"
              >
                {{ student.name }} ({{ student.group?.name }})
              </option>
            </select>
          </div>

          <div class="form-group">
            <label>Предмет *</label>
            <select v-model="gradeForm.subjectId" required :disabled="!!editingGrade">
              <option value="">Выберите предмет</option>
              <option 
                v-for="subject in subjects" 
                :key="subject.id" 
                :value="subject.id"
              >
                {{ subject.name }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label>Оценка (0-100 баллов) *</label>
            <div class="grade-input-container">
              <input 
                v-model.number="gradeForm.examScore" 
                type="number" 
                min="0" 
                max="100" 
                required
                class="grade-input"
              >
              <div class="grade-preview">
                <span :class="['preview-badge', getGradeClass(gradeForm.examScore)]">
                  {{ gradeForm.examScore || '--' }}
                </span>
                <div class="grade-hint" v-if="gradeForm.examScore >= 0">
                  {{ getGradeDescription(gradeForm.examScore) }}
                </div>
              </div>
            </div>
          </div>

          <div class="form-actions">
            <button type="button" @click="closeModal" class="cancel-btn">
              Отмена
            </button>
            <button 
              type="submit" 
              :disabled="isSaving || !isFormValid"
              class="submit-btn"
            >
              <span v-if="isSaving">Сохранение...</span>
              <span v-else>{{ editingGrade ? 'Обновить' : 'Сохранить' }}</span>
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Подтверждение удаления -->
    <div v-if="showDeleteConfirm" class="modal-overlay" @click.self="showDeleteConfirm = false">
      <div class="confirm-modal">
        <h3>Подтверждение удаления</h3>
        <p>Вы уверены, что хотите удалить эту оценку?</p>
        <div class="confirm-actions">
          <button @click="showDeleteConfirm = false" class="cancel-btn">
            Отмена
          </button>
          <button @click="confirmDelete" class="delete-confirm-btn">
            Удалить
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, reactive } from 'vue'
import { useAuthStore } from '@/Stores/auth'
import api, { type ExamDto, type CreateExamDto } from '@/services/api'

const auth = useAuthStore()

// Состояния
const grades = ref<ExamDto[]>([])
const students = ref<any[]>([])
const subjects = ref<any[]>([])
const isLoading = ref(false)
const isSaving = ref(false)
const error = ref<string | null>(null)

// Фильтры
const selectedStudentId = ref('')
const selectedSubjectId = ref('')

// Модальные окна
const showAddGradeModal = ref(false)
const editingGrade = ref<ExamDto | null>(null)
const showDeleteConfirm = ref(false)
const gradeToDelete = ref<number | null>(null)

// Форма оценки
const gradeForm = reactive({
  studentId: 0,
  subjectId: 0,
  examScore: 0,
  teacherId: auth.user?.teacherId || 0
})

// Вычисляемые свойства
const filteredGrades = computed(() => {
  let filtered = grades.value
  
  if (selectedStudentId.value) {
    filtered = filtered.filter(g => g.student.id === parseInt(selectedStudentId.value))
  }
  
  if (selectedSubjectId.value) {
    filtered = filtered.filter(g => g.subject.id === parseInt(selectedSubjectId.value))
  }
  
  return filtered.sort((a, b) => {
    if (a.student.name !== b.student.name) {
      return a.student.name.localeCompare(b.student.name)
    }
    return a.subject.name.localeCompare(b.subject.name)
  })
})

const averageGrade = computed(() => {
  if (filteredGrades.value.length === 0) return '0.00'
  const sum = filteredGrades.value.reduce((acc, grade) => acc + grade.examScore, 0)
  return (sum / filteredGrades.value.length).toFixed(2)
})

const uniqueStudents = computed(() => {
  const studentIds = new Set(filteredGrades.value.map(g => g.student.id))
  return studentIds.size
})

const isFormValid = computed(() => {
  return gradeForm.studentId > 0 && 
         gradeForm.subjectId > 0 && 
         gradeForm.examScore >= 0 && 
         gradeForm.examScore <= 100
})

// Методы
const loadData = async () => {
  isLoading.value = true
  error.value = null
  
  try {
    await Promise.all([
      loadGrades(),
      loadStudents(),
      loadSubjects()
    ])
  } catch (err: any) {
    error.value = 'Ошибка загрузки данных. Проверьте подключение к серверу.'
    console.error('Ошибка загрузки данных:', err)
  } finally {
    isLoading.value = false
  }
}

const loadGrades = async () => {
  try {
    if (auth.user?.teacherId) {
      // Используйте endpoint, который возвращает данные с группами
      grades.value = await api.getTeacherGrades(auth.user.teacherId)
    }
  } catch (err: any) {
    console.error('Ошибка загрузки оценок:', err)
    error.value = 'Не удалось загрузить оценки'
  }
}

// Временный метод до реализации специального endpoint
const getAllGrades = async (): Promise<ExamDto[]> => {
  // Получаем оценки всех студентов (нужно реализовать на бэкенде)
  // Создайте endpoint GET /api/exams
  try {
    // Пока возвращаем пустой массив - данные будут приходить с бэкенда
    return []
  } catch (err) {
    console.error('Ошибка получения всех оценок:', err)
    return []
  }
}

const loadStudents = async () => {
  try {
    // Используйте обновленный метод, который возвращает студентов с группами
    students.value = await api.getStudentsWithGroups()
  } catch (err: any) {
    console.error('Ошибка загрузки студентов:', err)
    error.value = 'Не удалось загрузить список студентов'
  }
}

const loadSubjects = async () => {
  try {
    subjects.value = await api.getSubjects()
  } catch (err: any) {
    console.error('Ошибка загрузки предметов:', err)
    error.value = 'Не удалось загрузить список предметов'
  }
}

const getGradeClass = (score: number) => {
  if (score >= 85) return 'grade-excellent'
  if (score >= 70) return 'grade-good'
  if (score >= 60) return 'grade-medium'
  return 'grade-poor'
}

const getGradeDescription = (score: number) => {
  if (score >= 85) return 'Отлично'
  if (score >= 70) return 'Хорошо'
  if (score >= 60) return 'Удовлетворительно'
  if (score > 0) return 'Неудовлетворительно'
  return 'Введите оценку'
}

const editGrade = (grade: ExamDto) => {
  editingGrade.value = grade
  gradeForm.studentId = grade.student.id
  gradeForm.subjectId = grade.subject.id
  gradeForm.examScore = grade.examScore
  showAddGradeModal.value = true
}

const deleteGrade = (id: number) => {
  gradeToDelete.value = id
  showDeleteConfirm.value = true
}

const confirmDelete = async () => {
  if (!gradeToDelete.value) return
  
  try {
    await api.deleteExam(gradeToDelete.value)
    // Удаляем из локального массива
    grades.value = grades.value.filter(g => g.id !== gradeToDelete.value)
    showDeleteConfirm.value = false
    gradeToDelete.value = null
  } catch (err: any) {
    error.value = 'Ошибка удаления оценки'
    console.error(err)
  }
}

const saveGrade = async () => {
  if (!isFormValid.value) {
    error.value = 'Заполните все поля правильно'
    return
  }
  
  isSaving.value = true
  error.value = null
  
  try {
    const gradeData: CreateExamDto = {
      studentId: gradeForm.studentId,
      teacherId: auth.user?.teacherId || 1,
      subjectId: gradeForm.subjectId,
      examScore: gradeForm.examScore
    }
    
    if (editingGrade.value) {
      // Обновление существующей оценки
      // Этот endpoint нужно добавить на бэкенде:
      // PUT /api/exams/{id}
      await api.updateExam(editingGrade.value.id, gradeData)
      // Перезагружаем данные
      await loadGrades()
    } else {
      // Добавление новой оценки
      await api.addGrade(gradeData)
      await loadGrades()
    }
    
    closeModal()
  } catch (err: any) {
    error.value = err.response?.data || 'Ошибка сохранения оценки'
    console.error(err)
  } finally {
    isSaving.value = false
  }
}

const closeModal = () => {
  showAddGradeModal.value = false
  editingGrade.value = null
  resetForm()
}

const resetForm = () => {
  gradeForm.studentId = 0
  gradeForm.subjectId = 0
  gradeForm.examScore = 0
}

const resetFilters = () => {
  selectedStudentId.value = ''
  selectedSubjectId.value = ''
}

// Загрузка данных при монтировании
onMounted(() => {
  if (auth.isTeacher) {
    loadData()
  }
})
</script>

<style scoped>
/* Стили остаются без изменений, как в предыдущем варианте */
.teacher-grades-page {
  background: white;
  border-radius: 10px;
  padding: 2rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid #f0f0f0;
}

.add-btn {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 8px;
  cursor: pointer;
  font-weight: bold;
  font-size: 1rem;
  transition: transform 0.2s, box-shadow 0.2s;
}

.add-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

.filters {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  padding: 1.5rem;
  background: #f8f9fa;
  border-radius: 8px;
  align-items: flex-end;
}

.filter-group {
  flex: 1;
  min-width: 200px;
}

.filter-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #495057;
}

.filter-group select {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ced4da;
  border-radius: 6px;
  font-size: 1rem;
  background: white;
  cursor: pointer;
}

.filter-btn, .reset-btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.2s;
  height: fit-content;
}

.filter-btn {
  background: #28a745;
  color: white;
}

.reset-btn {
  background: #6c757d;
  color: white;
}

.filter-btn:hover {
  background: #218838;
}

.reset-btn:hover {
  background: #5a6268;
}

.stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
}

.stat-card {
  background: linear-gradient(135deg, #667eea15 0%, #764ba215 100%);
  border: 1px solid #667eea30;
  border-radius: 8px;
  padding: 1.5rem;
  text-align: center;
}

.stat-value {
  font-size: 2.5rem;
  font-weight: bold;
  color: #667eea;
  margin-bottom: 0.5rem;
}

.stat-label {
  font-size: 0.9rem;
  color: #6c757d;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.loading {
  text-align: center;
  padding: 3rem;
}

.spinner {
  width: 50px;
  height: 50px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #667eea;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 1rem;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.error {
  background: #f8d7da;
  color: #721c24;
  padding: 1rem;
  border-radius: 6px;
  margin-bottom: 1rem;
  text-align: center;
}

.grades-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 1rem;
}

.grades-table th {
  background: #667eea;
  color: white;
  padding: 1rem;
  text-align: left;
  font-weight: 600;
  position: sticky;
  top: 0;
}

.grades-table td {
  padding: 1rem;
  border-bottom: 1px solid #e9ecef;
  vertical-align: middle;
}

.grade-badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-weight: bold;
  min-width: 40px;
  text-align: center;
}

.grade-excellent {
  background: #d4edda;
  color: #155724;
}

.grade-good {
  background: #d1ecf1;
  color: #0c5460;
}

.grade-medium {
  background: #fff3cd;
  color: #856404;
}

.grade-poor {
  background: #f8d7da;
  color: #721c24;
}

.actions {
  display: flex;
  gap: 0.5rem;
}

.edit-btn, .delete-btn {
  width: 36px;
  height: 36px;
  border: none;
  border-radius: 50%;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.2rem;
  transition: all 0.2s;
}

.edit-btn {
  background: #e3f2fd;
  color: #1976d2;
}

.delete-btn {
  background: #ffebee;
  color: #d32f2f;
}

.edit-btn:hover {
  background: #bbdefb;
  transform: scale(1.1);
}

.delete-btn:hover {
  background: #ffcdd2;
  transform: scale(1.1);
}

.empty-state {
  text-align: center;
  padding: 4rem;
  color: #6c757d;
}

.add-first-btn {
  margin-top: 1rem;
  padding: 0.75rem 2rem;
  background: #667eea;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.2s;
}

.add-first-btn:hover {
  background: #764ba2;
  transform: translateY(-2px);
}

/* Модальные окна */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0,0,0,0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
  backdrop-filter: blur(4px);
}

.modal {
  background: white;
  padding: 2.5rem;
  border-radius: 12px;
  min-width: 500px;
  max-width: 90%;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 10px 30px rgba(0,0,0,0.2);
}

.modal h3 {
  margin-bottom: 1.5rem;
  color: #333;
  font-size: 1.5rem;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #495057;
}

.form-group select,
.form-group input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ced4da;
  border-radius: 6px;
  font-size: 1rem;
  transition: border-color 0.2s;
}

.form-group select:focus,
.form-group input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.grade-input-container {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.grade-input {
  flex: 1;
}

.grade-preview {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.25rem;
}

.preview-badge {
  width: 60px;
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  font-size: 1.5rem;
  font-weight: bold;
}

.grade-hint {
  font-size: 0.8rem;
  color: #6c757d;
  text-align: center;
}

.disabled-input {
  background: #f8f9fa;
  cursor: not-allowed;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 2rem;
  padding-top: 1.5rem;
  border-top: 1px solid #e9ecef;
}

.cancel-btn, .submit-btn {
  padding: 0.75rem 2rem;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.2s;
  min-width: 120px;
}

.cancel-btn {
  background: #f8f9fa;
  color: #495057;
}

.cancel-btn:hover {
  background: #e9ecef;
}

.submit-btn {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.submit-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

.submit-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.confirm-modal {
  background: white;
  padding: 2rem;
  border-radius: 12px;
  min-width: 400px;
  text-align: center;
}

.confirm-modal h3 {
  color: #dc3545;
  margin-bottom: 1rem;
}

.confirm-modal p {
  color: #6c757d;
  margin-bottom: 2rem;
}

.confirm-actions {
  display: flex;
  justify-content: center;
  gap: 1rem;
}

.delete-confirm-btn {
  padding: 0.75rem 2rem;
  background: #dc3545;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.2s;
}

.delete-confirm-btn:hover {
  background: #c82333;
  transform: translateY(-2px);
}
</style>