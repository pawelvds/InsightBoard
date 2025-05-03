import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import Login from './pages/Login'
import Register from './pages/Register'
import Dashboard from './pages/Dashboard'
import { Toaster} from "sonner";
import PublicProfile from "@/pages/PublicProfile";

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route path="/dashboard" element={<Dashboard />} />
                <Route path="*" element={<div className="p-4 text-center">404 Not Found</div>} />
                <Route path="/user/:username" element={<PublicProfile />} />
            </Routes>
            <Toaster richColors position="top-center" />
        </Router>
    )
}

export default App