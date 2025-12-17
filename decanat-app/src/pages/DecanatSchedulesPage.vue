<template>
  <div class="decanat-schedules-page">
    <div class="page-header">
      <h2>Управление расписанием</h2>
      <button class="add-btn" @click="showAddModal = true">
        + Добавить занятие
      </button>
    </div>

    <div class="filters">
      <div class="filter-group">
        <label>Группа:</label>
        <select v-model="selectedGroupId">
          <option value="">Все группы</option>
          <option v-for="group in groups" :key="group.id" :value="group.id">
            {{ group.name }}
          </option>
        </select>
      </div>
      
      <div class="filter-group">
        <label>Дата:</label>
        <input type="date" v-model="selectedDate">
      </div>
      
      <button class="filter-btn" @click="loadSchedules">
        Применить
      </button>
    </div>

    <div v-if="isLoading" class="loading">
      Загрузка расписания...
    </div>

    <div v-else-if="error" class="error">
      {{ error }}
    </div>

    <div v-else class="schedules-container">
      <table class="schedules-table">
        <thead>
          <tr>
            <th>Дата</th>
            <th>Пара</th>
            <th>Группа</th>
            <th>Предмет</th>
            <th>Преподаватель</th>
            <th>Аудитория</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="schedule in filteredSchedules" :key="schedule.id">
            <td>{{ formatDate(schedule.date) }}</td>
            <td>{{ schedule.pairNumber }}</td>
            <td>{{ schedule.group.name }}</td>
            <td>{{ schedule.subject.name }}</td>
            <td>{{ schedule.teacher.name }}</td>
            <td>{{ schedule.classroom }}</td>
            <td class="actions">
              <button class="edit-btn" @click="editSchedule(schedule)">
                ✏️
              </button>
              <button class="delete-btn" @click="deleteSchedule(schedule.id)">
                🗑️
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Модальное окно добавления/редактирования -->
    <div v-if="showAddModal || editingSchedule" class="modal-overlay" @click.self="closeModal">
      <div class="modal">
        <h3>{{ editingSchedule ? 'Редактирование занятия' : 'Добавление занятия' }}</h3>
        <form @submit.prevent="saveSchedule">
          <div class="form-row">
            <div class="form-group">
              <label>Группа *</label>
              <select v-model="form.groupId" required>
                <option v-for="group in groups" :key="group.id" :value="group.id">
                  {{ group.name }}
                </option>
              </select>
            </div>
            
            <div class="form-group">
              <label>Преподаватель *</label>
              <select v-model="form.teacherId" required>
                <option v-for="teacher in teachers" :key="teacher.id" :value="teacher.id">
                  {{ teacher.name }}
                </option>
              </select>
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label>Предмет *</label>
              <select v-model="form.subjectId" required>
                <option v-for="subject in subjects" :key="subject.id" :value="subject.id">
                  {{ subject.name }}
                </option>
              </select>
            </div>
            
            <div class="form-group">
              <label>Аудитория *</label>
              <input v-model="form.classroom" type="text" required>
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label>Дата *</label>
              <input v-model="form.date" type="date" required>
            </div>
            
            <div class="form-group">
              <label>Номер пары *</label>
              <select v-model="form.pairNumber" required>
                <option v-for="n in 8" :key="n" :value="n">{{ n }}</option>
              </select>
            </div>
          </div>

          <div class="form-actions">
            <button type="button" @click="closeModal">Отмена</button>
            <button type="submit" :disabled="isSaving">
              {{ isSaving ? 'Сохранение...' : (editingSchedule ? 'Обновить' : 'Добавить') }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, reactive } from 'vue'
import api, { type ScheduleDto, type CreateScheduleDto } from '@/services/api'
import { useScheduleStore } from '@/Stores/schedule'

interface Group {
  id: number
  name: string
}

interface Teacher {
  id: number
  name: string
}

interface Subject {
  id: number
  name: string
  description: string
}

const scheduleStore = useScheduleStore()
const isLoading = ref(false)
const error = ref<string | null>(null)
const isSaving = ref(false)

const groups = ref<Group[]>([])
const teachers = ref<Teacher[]>([])
const subjects = ref<Subject[]>([])

const selectedGroupId = ref('')
const selectedDate = ref('')

const showAddModal = ref(false)
const editingSchedule = ref<ScheduleDto | null>(null)

const form = reactive({
  groupId: 0,
  teacherId: 0,
  subjectId: 0,
  date: '',
  pairNumber: 1,
  classroom: ''
})

// Фильтрация расписания
const filteredSchedules = computed(() => {
  let filtered = scheduleStore.schedules
  
  if (selectedGroupId.value) {
    filtered = filtered.filter(s => s.group.id === parseInt(selectedGroupId.value))
  }
  
  if (selectedDate.value) {
    filtered = filtered.filter(s => s.date === selectedDate.value)
  }
  
  return filtered.sort((a, b) => {
    if (a.date === b.date) {
      return a.pairNumber - b.pairNumber
    }
    return new Date(a.date).getTime() - new Date(b.date).getTime()
  })
})

// Методы
const loadData = async () => {
  isLoading.value = true
  error.value = null
  
  try {
    await Promise.all([
      loadSchedules(),
      loadGroups(),
      loadTeachers(),
      loadSubjects()
    ])
  } catch (err: any) {
    error.value = 'Ошибка загрузки данных'
    console.error(err)
  } finally {
    isLoading.value = false
  }
}

const loadSchedules = async () => {
  await scheduleStore.loadSchedules()
}

const loadGroups = async () => {
  groups.value = await api.getGroups()
}

const loadTeachers = async () => {
  teachers.value = await api.getTeachers()
}

const loadSubjects = async () => {
  subjects.value = await api.getSubjects()
}

const formatDate = (dateStr: string) => {
  return new Date(dateStr).toLocaleDateString('ru-RU')
}

const editSchedule = (schedule: ScheduleDto) => {
  editingSchedule.value = schedule
  Object.assign(form, {
    groupId: schedule.group.id,
    teacherId: schedule.teacher.id,
    subjectId: schedule.subject.id,
    date: schedule.date,
    pairNumber: schedule.pairNumber,
    classroom: schedule.classroom
  })
}

const deleteSchedule = async (id: number) => {
  if (!confirm('Удалить это занятие?')) return
  
  try {
    await scheduleStore.deleteSchedule(id)
  } catch (err: any) {
    error.value = 'Ошибка удаления занятия'
  }
}

const saveSchedule = async () => {
  if (!form.groupId || !form.teacherId || !form.subjectId || !form.date || !form.classroom) {
    alert('Заполните все обязательные поля')
    return
  }
  
  isSaving.value = true
  
  try {
    const scheduleData: CreateScheduleDto = {
      groupId: form.groupId,
      teacherId: form.teacherId,
      subjectId: form.subjectId,
      date: form.date,
      pairNumber: form.pairNumber,
      classroom: form.classroom
    }
    
    if (editingSchedule.value) {
      await api.updateSchedule(editingSchedule.value.id, scheduleData)
      await loadSchedules() // Перезагружаем список
    } else {
      await scheduleStore.createSchedule(scheduleData)
    }
    
    closeModal()
  } catch (err: any) {
    error.value = err.response?.data || 'Ошибка сохранения'
    console.error(err)
  } finally {
    isSaving.value = false
  }
}

const closeModal = () => {
  showAddModal.value = false
  editingSchedule.value = null
  resetForm()
}

const resetForm = () => {
  Object.assign(form, {
    groupId: 0,
    teacherId: 0,
    subjectId: 0,
    date: '',
    pairNumber: 1,
    classroom: ''
  })
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.decanat-schedules-page {
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
}

.add-btn {
  background: #667eea;
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 5px;
  cursor: pointer;
  font-weight: bold;
  transition: transform 0.2s;
}

.add-btn:hover {
  transform: translateY(-2px);
  background: #764ba2;
}

.filters {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  padding: 1rem;
  background: #f8f9fa;
  border-radius: 5px;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.filter-group label {
  font-size: 0.9rem;
  color: #666;
}

.filter-group select,
.filter-group input {
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 5px;
  min-width: 150px;
}

.filter-btn {
  align-self: flex-end;
  padding: 0.5rem 1rem;
  background: #28a745;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

.schedules-table {
  width: 100%;
  border-collapse: collapse;
}

.schedules-table th {
  background: #667eea;
  color: white;
  padding: 1rem;
  text-align: left;
}

.schedules-table td {
  padding: 1rem;
  border-bottom: 1px solid #e0e0e0;
}

.actions {
  display: flex;
  gap: 0.5rem;
}

.edit-btn, .delete-btn {
  padding: 0.25rem 0.5rem;
  border: none;
  border-radius: 3px;
  cursor: pointer;
  background: none;
  font-size: 1.2rem;
}

.edit-btn:hover {
  background: #e3f2fd;
}

.delete-btn:hover {
  background: #ffebee;
}

.loading, .error {
  text-align: center;
  padding: 3rem;
  color: #666;
}

.error {
  color: #e74c3c;
}

/* Модальное окно */
.modal {
  background: white;
  padding: 2rem;
  border-radius: 10px;
  min-width: 500px;
  max-width: 90%;
}

.form-row {
  display: flex;
  gap: 1rem;
  margin-bottom: 1rem;
}

.form-group {
  flex: 1;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  color: #555;
}

.form-group select,
.form-group input {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 5px;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 1.5rem;
}

.form-actions button {
  padding: 0.5rem 1.5rem;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-weight: bold;
}

.form-actions button[type="button"] {
  background: #f8f9fa;
  color: #333;
}

.form-actions button[type="submit"] {
  background: #667eea;
  color: white;
}
</style>